using BusinessObjects.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Utility.Common
{
    public static class JsonUtility 
    {
        public static async Task<ApiResponse<T>> DeserializeApiResponseAsync<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<T>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result ?? new ApiResponse<T>
            {
                Success = false,
                Message = "Empty or invalid response.",
                Data = default
            };
        }

        public static async Task<List<T>> DeserializeWrappedListAsync<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<ApiListWrapper<T>>>(json, options);

            return apiResponse?.Data?.Values ?? new List<T>();
        }

        public static async Task<T?> DeserializeApiResponseAsyncObj<T>(HttpResponseMessage response) where T : class
        {
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(json, options);
            return apiResponse?.Data;
        }

    }
}
