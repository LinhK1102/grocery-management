using System.Net.Http.Json;
using Utility.Common;
using WebApplication.Constants;
using WebApplication.Models;
using WebApplication.Models.Dto;
using WebApplication.Service;

namespace WebApplication.Services
{
    public class SupplierApiService : ApiClientService
    {
        public SupplierApiService(IHttpClientFactory factory, IHttpContextAccessor accessor, IConfiguration config)
            : base(factory, accessor, config) { }

        public async Task<List<SupplierDto>> GetAllAsync()
        {
            var res = await CreateClient().GetAsync(ApiRoutes.Suppliers.GetAll);
            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<List<SupplierDto>>(res);
            return apiResponse.Data ?? new();
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
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, SupplierDto dto)
        {
            var url = string.Format(ApiRoutes.Suppliers.Update, id);
            var res = await CreateClient().PutAsJsonAsync(url, dto);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var url = string.Format(ApiRoutes.Suppliers.Delete, id);
            var res = await CreateClient().DeleteAsync(url);
            return res.IsSuccessStatusCode;
        }
    }
}
