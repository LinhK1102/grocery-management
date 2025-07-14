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
using System;
using Repositories.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.OData;
using Utility.Mapper;
using Repositories.Manager;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);
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




// Thêm dịch vụ DbContext
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), 
    x => x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

builder.Services.Configure<ApiSettings>(builder.Configuration);

//auto mapping
builder.Services.AddAutoMapper(typeof(MappingProfile));

// --- DAOs: Direct database access ---
builder.Services.AddScoped<CustomerDAO>();
builder.Services.AddScoped<EmployeeDAO>();
builder.Services.AddScoped<OrderDAO>();
builder.Services.AddScoped<OrderDetailDAO>();
builder.Services.AddScoped<ProductDAO>();
builder.Services.AddScoped<RetailOutletDAO>();
builder.Services.AddScoped<SupplierDAO>();
builder.Services.AddScoped<WarehouseDAO>();
builder.Services.AddScoped<CategoryDAO>();
builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddScoped<ItemDAO>();
builder.Services.AddScoped<CategoryDAO>();
builder.Services.AddScoped<GoogleDriveService>();
builder.Services.AddScoped<GoogleAccessTokenService>();
builder.Services.AddScoped<CustomerRepository>();
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddScoped<ItemRepository>();
builder.Services.AddScoped<ProductRepository>(); // nếu có inject trực tiếp

// --- Repositories: Seed data management ---
builder.Services.AddScoped<SeedManager>();

// --- Repositories: Business logic layer ---
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
builder.Services.AddScoped<IGoogleSheetsService, GoogleSheetsService>();

builder.Services.AddScoped<InvoiceDAO>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();


// --- Repositories: Event handling ---
builder.Services.AddScoped<INotificationService, NotificationService>();

// --- Utilities: Supporting services ---
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

//HTTp client for Google Drive API
builder.Services.AddHttpClient<IBarcodeRepository, BarcodeRepository>();

// --- Hubs: Real-time communication ---
builder.Services.AddTransient(typeof(Lazy<>), typeof(LazyResolver<>));

IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<Employee>("Employees");
    return builder.GetEdmModel();
}

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


builder.Services.AddSignalR();

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
});

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

    // Thêm dòng sau để buộc Google hỏi lại quyền (rất quan trọng nếu user đã từng đăng nhập)
    options.AuthorizationEndpoint += "?prompt=consent&access_type=offline";
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient();

builder.WebHost.UseUrls("http://0.0.0.0:5100"); // mở cho tất cả IP máy

var app = builder.Build();

var baseDir = AppContext.BaseDirectory;

// Từ bin\Debug\netX.X → lên tới backend\GroceryUI\dist
var reactDistPath = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "GroceryUI", "dist"));

//if (!Directory.Exists(reactDistPath))
//{
//    Console.WriteLine("⚠️ 'dist' folder not found. Please build React app first.");
//    // Hoặc gọi npm run build tự động nếu muốn (như đã hướng dẫn trước)
//}
//else
//{
//    app.UseStaticFiles(new StaticFileOptions
//    {
//        OnPrepareResponse = ctx =>
//        {
//            ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store");
//            ctx.Context.Response.Headers.Append("Pragma", "no-cache");
//            ctx.Context.Response.Headers.Append("Expires", "-1");
//        }
//    });
//}

using (var scope = app.Services.CreateScope())
{
    var seedManager = scope.ServiceProvider.GetRequiredService<SeedManager>();
    await seedManager.SeedAllAsync();
}


app.UseCors(MyAllowSpecificOrigins);

// Sử dụng Swagger để test API
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Grocery API");
        c.RoutePrefix = "swagger";
    });
}

if (Directory.Exists(reactDistPath))
{
    app.UseStaticFiles();
}


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<NotificationHub>("/notificationHub");
app.MapControllers();

app.Run();
