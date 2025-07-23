using BusinessObjects.Entities;
using DataAccess.DAO;
using Utility.Hubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Repositories.Events;
using Repositories.Interfaces;
using Repositories.Repositories;
using BusinessObjects.Commons;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PdfSharpCore.Drawing.BarCodes;
using Repositories.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.OData;
using Utility.Mapper;
using Repositories.Manager;
using GroceryWebApp.Models.Dto;
using System.Net;
using BusinessObjects.DTOs;

var builder = WebApplication.CreateBuilder(args);

// CORS policy for allowing all (only for dev/test; not for production)
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// Add EF Core with SQL Server + optimized split query
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    x => x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

// Bind strongly typed config
builder.Services.Configure<ApiSettings>(builder.Configuration);

// Register AutoMapper with mapping profile
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register DAO classes
builder.Services.AddScoped<CustomerDAO>();
builder.Services.AddScoped<EmployeeDAO>();
builder.Services.AddScoped<OrderDAO>();
builder.Services.AddScoped<OrderDetailDAO>();
builder.Services.AddScoped<ProductDAO>();
builder.Services.AddScoped<RetailOutletDAO>();
builder.Services.AddScoped<SupplierDAO>();
builder.Services.AddScoped<WarehouseDAO>();
builder.Services.AddScoped<CategoryDAO>();
builder.Services.AddScoped<ItemDAO>();
builder.Services.AddScoped<InvoiceDAO>();

// Register manager/services
builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddScoped<GoogleDriveService>();
builder.Services.AddScoped<GoogleAccessTokenService>();
builder.Services.AddScoped<SeedManager>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Register Repositories
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IRetailOutletRepository, RetailOutletRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IBarcodeRepository, BarcodeRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IGoogleSheetsService, GoogleSheetsService>();

// Register HttpClient for barcode repository + general fallback
builder.Services.AddHttpClient<IBarcodeRepository, BarcodeRepository>();

builder.Services.AddHttpClient();

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// Dependency injection for lazy loading (e.g., SignalR hub)
builder.Services.AddTransient(typeof(Lazy<>), typeof(LazyResolver<>));

// Define OData EDM model
IEdmModel GetEdmModel()
{
    var modelBuilder = new ODataConventionModelBuilder();
    modelBuilder.EntitySet<ProductDto>("Products"); // Đổi từ Product → ProductDto
    modelBuilder.EntitySet<Employee>("Employees");
    return modelBuilder.GetEdmModel();
}

// Add MVC Controllers + OData + JSON loop handling
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
    })
    .AddOData(opt =>
    {
        opt.Select().Filter().Expand().OrderBy().Count().SetMaxTop(100)
            .AddRouteComponents("odata", GetEdmModel());
    });


// Add SignalR support
builder.Services.AddSignalR();

// Swagger documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Grocery API",
        Description = "API for Grocery Store System",
        Contact = new OpenApiContact
        {
            Name = "LinhTK",
            Email = "trankhanhlinh2201004@gmail.com"
        }
    });

    // JWT bearer definition
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Input: Bearer {your token here}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// JWT authentication config
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Google OAuth 2.0 authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.Scope.Add("https://www.googleapis.com/auth/drive.metadata.readonly");
    options.Scope.Add("https://www.googleapis.com/auth/drive.metadata");
    options.Scope.Add("https://www.googleapis.com/auth/spreadsheets");
    options.Scope.Add("https://www.googleapis.com/auth/drive.file");
    options.SaveTokens = true;
    options.AuthorizationEndpoint += "?prompt=consent&access_type=offline";
});

builder.Services.AddHttpContextAccessor();

// Start server on 0.0.0.0:5100 to accept external IP access
builder.WebHost.UseUrls("http://0.0.0.0:5100");

// Print dynamic IP address
var host = Dns.GetHostEntry(Dns.GetHostName());
var localIp = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString();
Console.WriteLine($"Access your app at http://{localIp}:5100/swagger/index.html");

var app = builder.Build();

// Static file setup for frontend React (dist folder)
var baseDir = AppContext.BaseDirectory;
var reactDistPath = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "GroceryUI", "dist"));
if (Directory.Exists(reactDistPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store");
            ctx.Context.Response.Headers.Append("Pragma", "no-cache");
            ctx.Context.Response.Headers.Append("Expires", "-1");
        }
    });
}

// Auto seed database
using (var scope = app.Services.CreateScope())
{
    var seedManager = scope.ServiceProvider.GetRequiredService<SeedManager>();
    await seedManager.SeedAllAsync();
}

// Enable Swagger only in Development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Grocery API");
        c.RoutePrefix = "swagger";
    });
}

// Enable HTTPS redirect
//app.UseHttpsRedirection();

// CORS middleware must be early in pipeline
app.UseCors(MyAllowSpecificOrigins);

// Enable authentication/authorization
app.UseAuthentication();
app.UseAuthorization();

// Setup SignalR Notification Hub
app.MapHub<NotificationHub>("/notificationHub");

// Setup controllers
app.MapControllers();

// Redirect root URL to Swagger page
app.MapGet("/", () => Results.Redirect("/swagger"));

// Start application
app.Run();
