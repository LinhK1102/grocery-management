using Azure;
using Humanizer;
using Microsoft.AspNetCore.SignalR;
using System.Net.Http.Json;
using Utility.Common;
using Utility.Hubs;
using WebApplication.Constants;
using WebApplication.Helpers;
using WebApplication.Models;
using WebApplication.Models.Dto;
using WebApplication.Service;

namespace WebApplication.Services
{
    public class SupplierApiService : ApiClientService
    {
        public SupplierApiService(IHttpClientFactory factory, IHttpContextAccessor accessor, IConfiguration config, IHubContext<NotificationHub> hubContext)
            : base(factory, accessor, config,hubContext) { }

        public async Task<List<SupplierDto>> GetAllAsync()
        {
            var res = await CreateClient().GetAsync(ApiRoutes.Suppliers.GetAll);
            var apiResponse = await JsonUtility.DeserializeWrappedListAsync<SupplierDto>(res);
            return apiResponse;
        }

        public async Task<SupplierDto?> GetByIdAsync(int id)
        {
            var url = string.Format(ApiRoutes.Suppliers.GetById, id);
            var res = await CreateClient().GetAsync(url);
            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<SupplierDto>(res);
            return apiResponse.Data;
        }

        public async Task<bool> CreateAsync(SupplierDto dto)
        {
            var res = await CreateClient().PostAsJsonAsync(ApiRoutes.Suppliers.Create, dto);
            return await NotifyAndReturnAsync(res, $"✅ Supplier '{dto.SupplierName}' created", $"❌ Failed to create supplier '{dto.SupplierName}'");
        }

        public async Task<bool> UpdateAsync(int id, SupplierDto dto)
        {
            var url = string.Format(ApiRoutes.Suppliers.Update, dto.SupplierId);
            var res = await CreateClient().PutAsJsonAsync(url, dto);
            //var apiResponse = await JsonUtility.DeserializeApiResponseAsync<SupplierDto>(res);
            if (!res.IsSuccessStatusCode)
            {
                var error = await res.Content.ReadAsStringAsync();
                Console.WriteLine($"Update failed: {error}");
                var content = await res.Content.ReadAsStringAsync();
                Console.WriteLine("❌ API Error: " + content); // thêm log này
                return false;
            }

            return await NotifyAndReturnAsync(res, $"✅ Supplier '{dto.SupplierName}' updated", $"❌ Failed to update supplier '{dto.SupplierName}'");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var url = string.Format(ApiRoutes.Suppliers.Delete, id);
            var res = await CreateClient().DeleteAsync(url);

            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<SupplierDto>(res);

            return await NotifyAndReturnAsync(res, $"✅ Supplier '{id}' deleted", $"❌ Failed to update supplier '{id}'");
        }

    }
}
