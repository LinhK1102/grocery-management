using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using WebApplication.Service;
using WebApplication.Services;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5101); // đúng ✅
});



// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddControllers()
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


// Cookie Authentication cho WebApp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // 👈 tự chuyển hướng nếu chưa đăng nhập
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.Configure<Microsoft.AspNetCore.Identity.IdentityOptions>(options =>
{
    options.ClaimsIdentity.RoleClaimType = System.Security.Claims.ClaimTypes.Role;
});

// Gọi API + lấy context
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

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
// ... các service khác


var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    //app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

// Đặt đúng thứ tự: UseRouting -> UseAuthentication/Authorization -> MapControllers
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}");

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.MapGet("/", context =>
{
    context.Response.Redirect("/Account/Login");
    return Task.CompletedTask;
});


app.Run();

