using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Services
{
    public class GoogleAccessTokenService
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public GoogleAccessTokenService(IConfiguration config, IHttpClientFactory factory)
        {
            _config = config;
            _httpClientFactory = factory;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var values = new Dictionary<string, string>
            {
                ["client_id"] = _config["GoogleAuth:ClientId"],
                ["client_secret"] = _config["GoogleAuth:ClientSecret"],
                ["refresh_token"] = _config["GoogleAuth:RefreshToken"],
                ["grant_type"] = "refresh_token"
            };

            var response = await client.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(values));
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get access token: {response.StatusCode} - {content}");
            }

            var json = JObject.Parse(content);

            return json["access_token"]?.ToString() ?? throw new Exception("Access token fetch failed");
        }
    }

}
