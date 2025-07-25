using Azure;
using GroceryWebApp.Constants;
using GroceryWebApp.Models.Dto;
using Microsoft.AspNetCore.SignalR;
using System.Net.Http.Json;
using System.Text.Json;
using Utility.Common;
using Utility.Hubs;

namespace GroceryWebApp.Service
{
    public class PaymentService : ApiClientService
    {
        public PaymentService(IHttpClientFactory factory, IHttpContextAccessor accessor, IConfiguration config, IHubContext<NotificationHub> hubContext)
            : base(factory, accessor, config, hubContext) { }

        public async Task<PayOSResponse> CreateAsync(CreateRequest createRequest)
        {
            var client = CreateClient();
            var url = ApiRoutes.Payment.Base;

            var response = await client.PostAsJsonAsync(url, createRequest);
            response.EnsureSuccessStatusCode();

            // 👉 thêm JsonSerializerOptions với PropertyNameCaseInsensitive = true
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var json = await response.Content.ReadAsStringAsync();
            try
            {
                Console.WriteLine(json);
                var result = JsonSerializer.Deserialize<PayOSResponse>(json, options);
                return result;
            }
            catch (Exception ex)
            {
                // log json và ex.Message
                Console.WriteLine("Deserialize error: " + ex.Message);
                Console.WriteLine("Raw json: " + json);
            }

            return null;

        }

    }
}
