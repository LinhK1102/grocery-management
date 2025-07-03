using System.Text.Json;
using WebApplication.Models;
using Microsoft.AspNetCore.Http;
using WebApplication.Service;
using Repositories.DTOs;
using Utility.Common;
using WebApplication.Constants;
using WebApplication.Models.Dto;

namespace WebApplication.Services
{
    public class AuthApiService : ApiClientService
    {
        public AuthApiService(IHttpClientFactory factory, IHttpContextAccessor accessor, IConfiguration config)
            : base(factory, accessor, config) { }

        public async Task<EmployeeDto?> LoginAsync(EmployeeLoginRequest request)
        {
            var client = CreateClient();
            var url = ApiRoutes.Auth.Login;
            var response = await client.PostAsJsonAsync(url, request);

            if (!response.IsSuccessStatusCode)
                return null;

            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<EmployeeDto>(response);
            return apiResponse.Data;
        }

        public async Task<bool> RegisterAsync(EmployeeRegisterRequest request)
        {
            var client = CreateClient();
            var url = ApiRoutes.Auth.Register;
            var response = await client.PostAsJsonAsync(url, request);
            return response.IsSuccessStatusCode;
        }
    }
}
