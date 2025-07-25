using Microsoft.AspNetCore.SignalR;
using System.Net.Http.Json;
using Utility.Common;
using Utility.Hubs;
using GroceryWebApp.Constants;
using GroceryWebApp.Models;
using GroceryWebApp.Models.Dto;
using GroceryWebApp.Service;
using System.Text.Json;

namespace GroceryWebApp.Services
{
    public class OrderApiService : ApiClientService
    {
        public OrderApiService(IHttpClientFactory factory, IHttpContextAccessor accessor, IConfiguration config, IHubContext<NotificationHub> hubContext)
            : base(factory, accessor, config, hubContext) { }

        public async Task<List<OrderDto>> GetAllAsync()
        {
            var res = await CreateClient().GetAsync(ApiRoutes.Orders.GetAll);
            var apiResponse = await JsonUtility.DeserializeWrappedListAsync<OrderDto>(res);
            return apiResponse;
        }

        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            var url = string.Format(ApiRoutes.Orders.GetById, id);
            var res = await CreateClient().GetAsync(url);
            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<OrderDto>(res);
            return apiResponse.Data;
        }

        public async Task<OrderDto> CreateAsync(OrderUpdateDto dto)
        {
            // Log JSON trước khi gửi
            var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine("=== Sending JSON to API ===");
            Console.WriteLine(json);

            var client = CreateClient();
            var response = await client.PostAsJsonAsync(ApiRoutes.Orders.Create, dto);

            // Log status code
            Console.WriteLine($"=== Response Status Code: {response.StatusCode} ===");

            // Log nội dung phản hồi
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("=== Response Body ===");
            Console.WriteLine(responseContent);

            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<OrderDto>(response);

            return apiResponse.Data;
        }


        public async Task<bool> UpdateAsync(int id, OrderDto dto)
        {
            var url = string.Format(ApiRoutes.Orders.Update, id);
            var res = await CreateClient().PutAsJsonAsync(url, dto);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var url = string.Format(ApiRoutes.Orders.Delete, id);
            var res = await CreateClient().DeleteAsync(url);
            return res.IsSuccessStatusCode;
        }

        public async Task<List<OrderDto>> SearchAsync(string keyword)
        {
            var url = string.Format(ApiRoutes.Orders.Search, keyword);
            var res = await CreateClient().GetAsync(url);
            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<List<OrderDto>>(res);
            return apiResponse.Data ?? new();
        }
    }
}

