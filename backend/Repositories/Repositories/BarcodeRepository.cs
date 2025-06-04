using Azure.Core;
using BusinessObjects.DTOs;
using Microsoft.Extensions.Configuration;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class BarcodeRepository : IBarcodeRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public BarcodeRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["UpcApi:ApiKey"]; // Lấy config từ API project
        }

        public async Task<UpcProductResponse?> GetProductByBarcodeAsync(string barcode)
        {
            try
            {
                var url = $"https://api.upcdatabase.org/product/{barcode}?apikey={_apiKey}";
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;

                using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                var root = doc.RootElement;

                string GetString(string name) =>
                    root.TryGetProperty(name, out var prop) && prop.ValueKind == JsonValueKind.String
                        ? prop.GetString()!
                        : "";

                var images = root.TryGetProperty("images", out var imgProp) && imgProp.ValueKind == JsonValueKind.Array
                    ? imgProp.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.String).Select(x => x.GetString()!).ToList()
                    : new List<string>();

                var stores = root.TryGetProperty("stores", out var storeProp) && storeProp.ValueKind == JsonValueKind.Array
                    ? storeProp.EnumerateArray().Select(store =>
                    {
                        var storeName = store.TryGetProperty("store", out var s) && s.ValueKind == JsonValueKind.String ? s.GetString()! : "";
                        var price = store.TryGetProperty("price", out var p) && p.ValueKind == JsonValueKind.String ? p.GetString()! : "";
                        var urlVal = store.EnumerateObject().FirstOrDefault(o => o.Name.StartsWith("http") && o.Value.ValueKind == JsonValueKind.String).Value.GetString() ?? "";
                        return new StoreInfo { Store = storeName, Price = price, Url = urlVal };
                    }).ToList()
                    : new List<StoreInfo>();

                ReviewInfo? reviews = null;
                if (root.TryGetProperty("reviews", out var revProp) && revProp.ValueKind == JsonValueKind.Object)
                {
                    reviews = new ReviewInfo
                    {
                        Thumbsup = revProp.TryGetProperty("thumbsup", out var up) && up.ValueKind == JsonValueKind.Number ? up.GetInt32() : 0,
                        Thumbsdown = revProp.TryGetProperty("thumbsdown", out var down) && down.ValueKind == JsonValueKind.Number ? down.GetInt32() : 0,
                    };
                }

                return new UpcProductResponse
                {
                    Status = root.TryGetProperty("success", out var success) && success.ValueKind == JsonValueKind.True,
                    Barcode = GetString("barcode"),
                    Title = GetString("title"),
                    Alias = GetString("alias"),
                    Description = GetString("description"),
                    Brand = GetString("brand"),
                    Manufacturer = GetString("manufacturer"),
                    Mpn = GetString("mpn"),
                    Msrp = GetString("msrp"),
                    ASIN = GetString("ASIN"),
                    Category = GetString("category"),
                    Images = images,
                    Stores = stores,
                    Reviews = reviews
                };
            }
            catch
            {
                return null;
            }
        }


    }
}
