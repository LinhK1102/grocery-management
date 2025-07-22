using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Utility.Hubs;
using GroceryWebApp.Models;

namespace GroceryWebApp.Service
{
    public class ApiClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ApiClientService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration config,
            IHubContext<NotificationHub> hubContext)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _config = config;
            _hubContext = hubContext;
        }

        public HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config["ApiBaseUrl"]);

            var token = _httpContextAccessor.HttpContext?.User?.FindFirst("Token")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        protected async Task<bool> NotifyAndReturnAsync(HttpResponseMessage res, string successMsg, string failMsg)
        {
            bool success = res.IsSuccessStatusCode;
            string message = success ? successMsg : failMsg;

            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);

            if (!success)
            {
                var error = await res.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ API Error: {error}");
            }

            return success;
        }

    }
}
