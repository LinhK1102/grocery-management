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

namespace Repositories.Services
{
    public class GoogleDriveService
    {
        private readonly GoogleAccessTokenService _tokenService;
        private readonly NetHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _contextAccessor;

        public GoogleDriveService(GoogleAccessTokenService tokenService, NetHttpClientFactory factory, IHttpContextAccessor contextAccessor)
        {
            _tokenService = tokenService;
            _httpClientFactory = factory;
            _contextAccessor = contextAccessor;
        }

        public async Task<StorageQuotaDto> GetStorageQuotaAsync()
        {
            var accessToken = await _tokenService.GetAccessTokenAsync();

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync("https://www.googleapis.com/drive/v3/about?fields=storageQuota");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var quota = JObject.Parse(content)["storageQuota"];

            long GetLong(JToken? token) => token?.Value<long>() ?? 0;
            double ToGB(long b) => Math.Round(b / 1_073_741_824.0, 2);

            var limit = GetLong(quota["limit"]);
            var usage = GetLong(quota["usage"]);
            var usageInDrive = GetLong(quota["usageInDrive"]);
            var usageInTrash = GetLong(quota["usageInDriveTrash"]);
            var remaining = limit > usage ? limit - usage : 0;

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

        public async Task<string> CreateSheetInFolderAsync(string folderId, string sheetName)
        {
            var accessToken = await _tokenService.GetAccessTokenAsync();

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var metadata = new
            {
                name = sheetName,
                mimeType = UtitlityConstant.Google_Drive_MimeType_Sheet,
                parents = new[] { folderId }
            };

            var content = new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(metadata),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("https://www.googleapis.com/drive/v3/files", content);
            response.EnsureSuccessStatusCode();

            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
            return json["id"]?.ToString() ?? throw new Exception("Failed to retrieve Sheet ID.");
        }

        public async Task<bool> UpdateLastRowAsync(string spreadsheetId, string sheetName, IList<object> rowData)
        {
            var accessToken = await _tokenService.GetAccessTokenAsync();

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Đọc tất cả các dòng để lấy số dòng hiện tại
            var readUrl = $"{UtitlityConstant.Google_Sheet_SheetsBaseUrl}/{spreadsheetId}/values/{sheetName}!{UtitlityConstant.Google_Sheet_DefaultRange}";
            var readResponse = await client.GetAsync(readUrl);
            readResponse.EnsureSuccessStatusCode();

            var readJson = JObject.Parse(await readResponse.Content.ReadAsStringAsync());
            int currentRow = readJson["values"]?.Count() ?? 0;

            var updateRange = $"{sheetName}!A{currentRow}";
            var body = new
            {
                range = updateRange,
                majorDimension = UtitlityConstant.Google_Sheet_MajorDimension,
                values = new[] { rowData }
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(body),
                Encoding.UTF8,
                "application/json"
            );

            var updateUrl = $"{UtitlityConstant.Google_Sheet_SheetsBaseUrl}/{spreadsheetId}/values/{updateRange}?valueInputOption={UtitlityConstant.Google_Sheet_ValueInputOption}";
            var updateResponse = await client.PutAsync(updateUrl, content);
            return updateResponse.IsSuccessStatusCode;
        }

    }
}
