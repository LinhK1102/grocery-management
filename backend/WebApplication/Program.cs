using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using GroceryWebApp.Service;
using GroceryWebApp.Services;
using Microsoft.AspNetCore.Authentication;
using Utility.Common;
using Utility.Hubs;
using GroceryWebApp.Helpers;

var builder = WebApplication.CreateBuilder(args);

//  Kestrel allow connect from other devices (Wifi,LAN)
// ✅ Cho phép mọi thiết bị truy cập qua cổng 5101 (LAN + localhost)
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5101); // Bao gồm cả 127.0.0.1 và mạng LAN
    serverOptions.ListenAnyIP(5102, listenOptions =>
    {
        listenOptions.UseHttps(); // Dùng dev certificate nếu có
    });
});

// Log IP
var host = Dns.GetHostEntry(Dns.GetHostName());
var localIp = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString();

var httpPort = 5101;
var httpsPort = 5102;

Console.WriteLine($"Login app via:");
Console.WriteLine($"- HTTP : http://{localIp}:{httpPort} OR http://localhost:{httpPort}");
Console.WriteLine($"- HTTPS: https://{localIp}:{httpsPort} OR https://localhost:{httpsPort}");


// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddLogging();

// Get IP LAN (IPv4)
var lanIp = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?.ToString();
var apiBase = $"http://{lanIp}:5100/"; // ✅ Sử dụng IP thật
builder.Configuration["ApiBaseUrl"] = apiBase; // ✅ Optional: ghi đè để dùng ở chỗ khác
Console.WriteLine($"API base address: {apiBase}");

// Đăng ký HttpClient dùng IP động
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri(apiBase);
});

builder.Services.AddHttpContextAccessor();

// Cookie Authentication cho WebApp
// ✅ Chỉ giữ 1 lần đăng ký Cookie Auth
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";              // hoặc "/login"
        options.LogoutPath = "/Account/Logout";            // hoặc "/logout"
        options.AccessDeniedPath = "/Account/AccessDenied"; // hoặc "/access-denied"

        options.Events.OnValidatePrincipal = async context =>
        {
            var token = context.Principal?.FindFirst("Token")?.Value;

            if (string.IsNullOrWhiteSpace(token))
            {
                Console.WriteLine("Token not exist or run-out time → logout");
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        };

    });

builder.Services.Configure<Microsoft.AspNetCore.Identity.IdentityOptions>(options =>
{
    options.ClaimsIdentity.RoleClaimType = System.Security.Claims.ClaimTypes.Role;
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new ValuesWrapperConverterFactory());
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .Select(e => new {
                    Field = e.Key,
                    Errors = e.Value.Errors.Select(err => err.ErrorMessage).ToArray()
                });

            return new BadRequestObjectResult(new { message = "Validation failed", errors });
        };
    });

builder.Services.AddScoped<ApiClientService>();
builder.Services.AddScoped<AuthApiService>();
builder.Services.AddScoped<BarcodeApiService>();
builder.Services.AddScoped<CustomerApiService>();
builder.Services.AddScoped<EmployeeApiService>();
builder.Services.AddScoped<OrderApiService>();
builder.Services.AddScoped<OrderDetailApiService>();
builder.Services.AddScoped<ProductApiService>();
builder.Services.AddScoped<RetailOutletApiService>();
builder.Services.AddScoped<SupplierApiService>();
builder.Services.AddScoped<WarehouseApiService>();
builder.Services.AddScoped<CategoryApiService>();
builder.Services.AddScoped<InvoiceApiService>();
builder.Services.AddScoped<InvoiceItemApiService>();
builder.Services.AddScoped<PaymentService>();
// ... các service khác

var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    //app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Đặt đúng thứ tự: UseRouting -> UseAuthentication/Authorization -> MapControllers
app.UseRouting();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<NotificationHub>("/notificationHub");
app.MapControllers(); 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}");

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.MapGet("/", context =>
{
    context.Response.Redirect("/login");
    return Task.CompletedTask;
});

app.Run();

