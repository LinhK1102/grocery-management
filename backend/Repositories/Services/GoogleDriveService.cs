using NetHttpClientFactory = System.Net.Http.IHttpClientFactory;
using GoogleHttpClientFactory = Google.Apis.Http.IHttpClientFactory;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using BusinessObjects.DTOs;
using Google.Apis.Http;
using Utility.Common;
using Newtonsoft.Json;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;

namespace Repositories.Services
{
    public class GoogleDriveService
    {
        private readonly GoogleAccessTokenService _tokenService;
        private readonly NetHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _config;

        public GoogleDriveService(IConfiguration config, GoogleAccessTokenService tokenService, NetHttpClientFactory factory, IHttpContextAccessor contextAccessor)
        {
            _tokenService = tokenService;
            _httpClientFactory = factory;
            _contextAccessor = contextAccessor;
            _config = config;
        }

        public async Task<StorageQuotaDto> GetStorageQuotaAsync()
        {
            try
            {
                // 1. Lấy đường dẫn file key từ appsettings.json
                var webAppUrl = _config["GoogleWebApp:Url"]; // Lấy URL từ appsettings.json
                var client = _httpClientFactory.CreateClient();

                var payload = new { action = "getStorageQuota" };
                var jsonContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(webAppUrl, jsonContent);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(jsonString);

                if (jsonResponse["status"]?.ToString() == "error")
                {
                    throw new Exception($"WebApp returned an error: {jsonResponse["message"]}");
                }

                var quotaData =  jsonResponse["data"];

                // 5. Ánh xạ dữ liệu trực tiếp từ đối tượng trả về
                double ToGB(long? b) => Math.Round((b ?? 0) / 1_073_741_824.0, 2);
                long limit = quotaData["limit"]?.Value<long>() ?? 0;
                long usage = quotaData["usage"]?.Value<long>() ?? 0;
                long usageInDrive = quotaData["usageInDrive"]?.Value<long>() ?? 0;
                long usageInTrash = quotaData["usageInDriveTrash"]?.Value<long>() ?? 0;
                long remaining = limit > usage ? limit - usage : 0;

                return new StorageQuotaDto
                {
                    Limit = limit,
                    Usage = usage,
                    UsageInDrive = usageInDrive,
                    UsageInDriveTrash = usageInTrash,
                    Remaining = remaining,
                    Limit_GB = ToGB(limit),
                    Usage_GB = ToGB(usage),
                    Remaining_GB = ToGB(remaining),
                    UsageInDrive_GB = ToGB(usageInDrive),
                    UsageInTrash_GB = ToGB(usageInTrash)
                };
            }
            catch (Exception ex)
            {
                // Ném ra lỗi cụ thể hơn để dễ dàng debug
                throw new Exception("An error occurred while getting Google Drive storage quota.", ex);
            }
        }

        public async Task<string> CreateSheetInFolderAsync(string folderId, string sheetName)
        {
            var webAppUrl = _config["GoogleWebApp:Url"];
            var client = _httpClientFactory.CreateClient();

            var payload = new { action = "createSheet", folderId, sheetName };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(webAppUrl, jsonContent);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var jsonResponse = JObject.Parse(jsonString);

            if (jsonResponse["status"]?.ToString() == "error")
            {
                throw new Exception($"WebApp returned an error: {jsonResponse["message"]}");
            }

            return jsonResponse["data"]?["id"]?.ToString() ?? throw new Exception("Failed to retrieve Sheet ID from WebApp response.");
        }

        public async Task<bool> UpdateLastRowAsync(string spreadsheetId, string sheetName, IList<object> rowData)
        {
            var webAppUrl = _config["GoogleWebApp:Url"];
            var client = _httpClientFactory.CreateClient();

            // Dùng action "appendRow" mới để thêm vào cuối
            var payload = new { action = "appendRow", spreadsheetId, sheetName, rowData };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(webAppUrl, jsonContent);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var jsonResponse = JObject.Parse(jsonString);

            return jsonResponse["status"]?.ToString() == "success";
        }
    }
}
