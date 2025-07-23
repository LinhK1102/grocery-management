using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using GroceryWebApp.Models;
using GroceryWebApp.Models.Account;
using GroceryWebApp.Views.Account;

namespace GroceryWebApp.Controllers.Authentication
{

    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public AccountController(IHttpClientFactory factory, IConfiguration config)
        {
            _httpClientFactory = factory;
            _config = config;
        }

        [HttpGet("login")]
        public async Task<IActionResult> LoginAsync()
         
        {
            if (User.Identity?.IsAuthenticated == true && !string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
                //return View("~/Views/Shared/AccessDenied.cshtml");
            }

            if (!await IsApiAvailable())
            {
                ViewBag.Error = "API hiện không khả dụng. Vui lòng thử lại sau.";
                return View();
            }

            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var apiBaseUrl = _config["ApiBaseUrl"];
            var client = _httpClientFactory.CreateClient("API"); 

            // 🔍 Kiểm tra API có đang hoạt động không
            try
            {
                var healthCheck = await client.GetAsync("/swagger/v1/swagger.json"); // hoặc "/health" nếu bạn có health endpoint
                if (!healthCheck.IsSuccessStatusCode)
                {
                    ViewBag.Error = $"Không thể kết nối đến API tại {apiBaseUrl}.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"API không khả dụng ({apiBaseUrl}). Vui lòng kiểm tra lại kết nối mạng hoặc cấu hình.";
                return View(model);
            }

            // Gửi login
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/Auth/login", content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = $"Login fail.Double check Email and/or Password.";
                return View(model);
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResult = JsonSerializer.Deserialize<ApiResponse<LoginResponse>>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var token = apiResult?.Data?.Token;
            var fullName = apiResult?.Data?.FullName;
            var role = apiResult?.Data?.Role ?? "User";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Email),
                new Claim("FullName", fullName ?? ""),
                new Claim(ClaimTypes.Role, role),
                new Claim("Token", token ?? "")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        private async Task<bool> IsApiAvailable()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("API"); 
                var response = await client.GetAsync("/swagger/v1/swagger.json");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        [HttpGet("access-denied")]
        public IActionResult AccessDenied()
        {
            return View("~/Views/Shared/AccessDenied.cshtml");
        }

    }
}
