using System.Text.Json;
using WebApplication.Models;
using Microsoft.AspNetCore.Http;
using WebApplication.Service;
using Utility.Common;
using WebApplication.Constants;
using WebApplication.Helpers;
using WebApplication.Models.Dto;

namespace WebApplication.Services
{
    public class WarehouseApiService : ApiClientService
    {
        public WarehouseApiService(IHttpClientFactory factory, IHttpContextAccessor accessor, IConfiguration config)
            : base(factory, accessor, config) { }

        public async Task<List<WarehouseDto>> GetAllAsync()
        {
            var res = await CreateClient().GetAsync(ApiRoutes.Warehouse.GetAll);
            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<List<WarehouseDto>>(res);
            return apiResponse.Data ?? new();
        }

        public async Task<WarehouseDto?> GetByIdAsync(int id)
        {
            var url = string.Format(ApiRoutes.Warehouse.GetById, id);
            var res = await CreateClient().GetAsync(url);
            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<WarehouseDto>(res);
            return apiResponse.Data;
        }

        public async Task<bool> CreateAsync(WarehouseDto dto)
        {
            var res = await CreateClient().PostAsJsonAsync(ApiRoutes.Warehouse.Create, dto);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, WarehouseDto dto)
        {
            var url = string.Format(ApiRoutes.Warehouse.Update, id);
            var res = await CreateClient().PutAsJsonAsync(url, dto);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var url = string.Format(ApiRoutes.Warehouse.Delete, id);
            var res = await CreateClient().DeleteAsync(url);
            return res.IsSuccessStatusCode;
        }
    }
}
