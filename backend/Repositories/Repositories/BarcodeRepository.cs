using Azure.Core;
using BusinessObjects.Commons;
using BusinessObjects.DTOs;
using BusinessObjects.Entities;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
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
using Utility.Mapper;

namespace Repositories.Repositories
{
    public class BarcodeRepository : IBarcodeRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly IProductRepository _productRepository;

        public BarcodeRepository(HttpClient httpClient, IConfiguration configuration, IProductRepository productRepository)
        {
            _httpClient = httpClient;
            _apiKey = configuration["UpcApi:ApiKey"]; // Lấy config từ API project
            _productRepository = productRepository;
        }

        public async Task<UpcProductResponse?> GetProductInfoFromApiAsync(string barcode)
        {
            try
            {
                var url = $"https://api.upcdatabase.org/product/{barcode}?apikey={_apiKey}";
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;

                //using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                var jsonString = await response.Content.ReadAsStringAsync();

                // Lưu file trước khi parse
                var filePath = @"D:\FPT\2025_Summer\PRN232\jsonValue.txt";
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                await File.WriteAllTextAsync(filePath, jsonString);

                // Sau đó parse bình thường
                using var doc = JsonDocument.Parse(jsonString);

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

        public async Task<ApiResponse<Product>> GetOrCreateProductByBarcodeAsync(string barcode)
        {
            // Bước 1: Kiểm tra sản phẩm đã có trong DB chưa
            var existingProduct = _productRepository.GetProductByBarcode(barcode);
            if (existingProduct != null)
            {
                return null;
            }

            // Bước 2: Gọi API ngoài để lấy thông tin sản phẩm
            var upcResponse = await GetProductInfoFromApiAsync(barcode);
            if (upcResponse == null || !upcResponse.Status)
            {
                return null; // hoặc throw lỗi tùy cách bạn xử lý
            }

            // Bước 3: Mapping thông tin từ response → entity
            var newProduct = UpcProductMapper.ToProductEntity(upcResponse);
             _productRepository.AddProduct(newProduct);


            return new ApiResponse<Product>
            {
                Message = "Product created successfully",
                Success = true,
                Data = newProduct
            };
        }

        Task<UpcProductResponse?> IBarcodeRepository.GetOrCreateProductByBarcodeAsync(string barcode)
        {
            throw new NotImplementedException();
        }
    }
}
