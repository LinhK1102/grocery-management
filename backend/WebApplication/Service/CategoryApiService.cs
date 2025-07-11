
using BusinessObjects.Entities;
using Microsoft.AspNetCore.SignalR;
using Utility.Common;
using Utility.Hubs;
using WebApplication.Constants;
using WebApplication.Models;
using WebApplication.Models.Dto;

namespace WebApplication.Service
{
    public class CategoryApiService : ApiClientService
    {
        public CategoryApiService(IHttpClientFactory factory, IHttpContextAccessor accessor, IConfiguration config, IHubContext<NotificationHub> hubContext)
            : base(factory, accessor, config, hubContext) { }
        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var response = await CreateClient().GetAsync(ApiRoutes.Category.GetAll);
            var apiResponse = await JsonUtility.DeserializeWrappedListAsync<CategoryDto>(response);
            return apiResponse;
        }
        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var url = string.Format(ApiRoutes.Category.GetById, id);
            var res = await CreateClient().GetAsync(url);
            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<CategoryDto>(res);

            // Fix: Access the 'Data' property of 'ApiResponse<CategoryDto>' to retrieve the actual 'CategoryDto' object.
            return apiResponse?.Data ?? new CategoryDto();
        }
        public async Task<bool> CreateAsync(CategoryDto dto)
        {
            var res = await CreateClient().PostAsJsonAsync(ApiRoutes.Category.Create, dto);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, CategoryDto dto)
        {
            var url = string.Format(ApiRoutes.Category.Update, id);
            var res = await CreateClient().PutAsJsonAsync(url, dto);
            return res.IsSuccessStatusCode;
        }


    }
}
