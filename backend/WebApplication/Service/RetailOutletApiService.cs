using System.Net.Http.Json;
using Utility.Common;
using WebApplication.Constants;
using WebApplication.Models;
using WebApplication.Models.Dto;
using WebApplication.Service;

namespace WebApplication.Services
{
    public class RetailOutletApiService : ApiClientService
    {
        public RetailOutletApiService(IHttpClientFactory factory, IHttpContextAccessor accessor, IConfiguration config)
            : base(factory, accessor, config) { }

        public async Task<List<RetailOutletDto>> GetAllAsync()
        {
            var res = await CreateClient().GetAsync(ApiRoutes.RetailOutlets.GetAll);
            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<List<RetailOutletDto>>(res);
            return apiResponse.Data ?? new();
        }

        public async Task<RetailOutletDto?> GetByIdAsync(int id)
        {
            var url = string.Format(ApiRoutes.RetailOutlets.GetById, id);
            var res = await CreateClient().GetAsync(url);
            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<RetailOutletDto>(res);
            return apiResponse.Data;
        }

        public async Task<bool> CreateAsync(RetailOutletDto dto)
        {
            var res = await CreateClient().PostAsJsonAsync(ApiRoutes.RetailOutlets.Create, dto);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, RetailOutletDto dto)
        {
            var url = string.Format(ApiRoutes.RetailOutlets.Update, id);
            var res = await CreateClient().PutAsJsonAsync(url, dto);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var url = string.Format(ApiRoutes.RetailOutlets.Delete, id);
            var res = await CreateClient().DeleteAsync(url);
            return res.IsSuccessStatusCode;
        }
    }

}
