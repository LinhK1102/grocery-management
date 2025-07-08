using BusinessObjects.DTOs;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Utility.Common;
using Microsoft.Extensions.Http;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace Repositories.Services
{
    public class GoogleDriveService
    {
        private readonly GoogleAccessTokenService _tokenService;
        private readonly IHttpClientFactory _httpClientFactory;

        public GoogleDriveService(GoogleAccessTokenService tokenService, IHttpClientFactory factory)
        {
            _tokenService = tokenService;
            _httpClientFactory = factory;
        }

        public async Task<StorageQuotaDto> GetStorageQuotaAsync()
        {
            var accessToken = await _tokenService.GetAccessTokenAsync();

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

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
    }

}
