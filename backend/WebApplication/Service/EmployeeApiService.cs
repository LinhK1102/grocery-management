using BusinessObjects.DTOs;
using Microsoft.AspNetCore.SignalR;
using Repositories.DTOs;
using System.Text.Json;
using Utility.Common;
using Utility.Hubs;
using WebApplication.Constants;
using WebApplication.Helpers;
using WebApplication.Models;
using WebApplication.Models.Dto;
using WebApplication.Service;

namespace WebApplication.Services
{
    public class EmployeeApiService : ApiClientService
    {
        public EmployeeApiService(IHttpClientFactory factory, IHttpContextAccessor accessor, IConfiguration config, IHubContext<NotificationHub> hubContext)
            : base(factory, accessor, config, hubContext) { }

        public async Task<List<EmployeeDto>> GetAllAsync()
        {
            var res = await CreateClient().GetAsync(ApiRoutes.Employee.GetAll);
            var apiResponse = await JsonUtility.DeserializeWrappedListAsync<EmployeeDto>(res);
            return apiResponse;
        }

        public async Task<EmployeeDto?> GetByIdAsync(int id)
        {
            var url = string.Format(ApiRoutes.Employee.GetById, id);
            var res = await CreateClient().GetAsync(url);
            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<EmployeeDto>(res);
            return apiResponse.Data;
        }

        public async Task<bool> CreateAsync(EmployeeRegisterRequest dto)
        {
            var res = await CreateClient().PostAsJsonAsync(ApiRoutes.Employee.Create, dto);
            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<EmployeeDto>(res);
            if (!apiResponse.Success)
                return false;
            Console.WriteLine($"apiResponse: {apiResponse.Success}\n\rMessage:{apiResponse.Message}\n\rData:{apiResponse.Data.EmployeeId}_{apiResponse.Data.EmployeeEmail}");
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, EmployeeDto dto)
        {
            var url = string.Format(ApiRoutes.Employee.Update, id);
            var res = await CreateClient().PutAsJsonAsync(url, dto);
            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<EmployeeDto>(res);
            Console.WriteLine($"apiResponse: {apiResponse.Success}\n\rMessage:{apiResponse.Message}\n\rData:{apiResponse.Data.EmployeeId}_{apiResponse.Data.EmployeeName}_{apiResponse.Data.EmployeeEmail}");
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var url = string.Format(ApiRoutes.Employee.Delete, id);
            var res = await CreateClient().DeleteAsync(url);
            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<EmployeeDto>(res);
            Console.WriteLine($"apiResponse: {apiResponse.Success}\n\rMessage:{apiResponse.Message}\n\rData:{apiResponse.Data}");
            return res.IsSuccessStatusCode;
        }

        public async Task<List<EmployeeDto>> GetTopSellersAsync()
        {
            var res = await CreateClient().GetAsync(ApiRoutes.Employee.TopSellers);
            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<List<EmployeeDto>>(res);
            return apiResponse.Data ?? new();
        }
    }

}
