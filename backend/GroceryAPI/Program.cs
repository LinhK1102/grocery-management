using BusinessObjects.Entities;
using DataAccess.DAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Repositories.Interfaces;
using Repositories.Repositories;

var builder = WebApplication.CreateBuilder(args);
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
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- DAOs: Direct database access ---
builder.Services.AddScoped<CustomerDAO>();
builder.Services.AddScoped<EmployeeDAO>();
builder.Services.AddScoped<OrderDAO>();
builder.Services.AddScoped<OrderDetailDAO>();
builder.Services.AddScoped<ProductDAO>();
builder.Services.AddScoped<RetailOutletDAO>();
builder.Services.AddScoped<SupplierDAO>();
builder.Services.AddScoped<WarehouseDAO>();


// --- Repositories: Business logic layer ---
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IRetailOutletRepository, RetailOutletRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();

// --- Utilities: Supporting services ---
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

//
builder.Services.AddHttpClient<IBarcodeRepository, BarcodeRepository>();
//builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var baseDir = AppContext.BaseDirectory;

// Từ bin\Debug\netX.X → lên tới backend\GroceryUI\dist
var reactDistPath = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "GroceryUI", "dist"));

if (!Directory.Exists(reactDistPath))
{
    Console.WriteLine("⚠️ 'dist' folder not found. Please build React app first.");
    // Hoặc gọi npm run build tự động nếu muốn (như đã hướng dẫn trước)
}
else
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(reactDistPath),
        RequestPath = ""
    });
}


app.MapFallbackToFile("index.html");

app.UseCors(MyAllowSpecificOrigins);

// Sử dụng Swagger để test API
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
