using Utility.Common;
using GroceryWebApp.Constants;
using GroceryWebApp.Models.Dto;
using GroceryWebApp.Service;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Utility.Hubs;
using System.Drawing.Printing;
using System.Net.Http;
using System.Text.Json.Serialization;
using BusinessObjects.Commons;

namespace GroceryWebApp.Services
{
    public class ProductApiService : ApiClientService
    {
        public ProductApiService(IHttpClientFactory factory, IHttpContextAccessor accessor, IConfiguration config, IHubContext<NotificationHub> hubContext)
            : base(factory, accessor, config, hubContext) { }

        public async Task<List<ProductDto>> GetAllAsync(string search = "", int page = 1, int pageSize = 10)
        {
            var client = CreateClient();
            var url = $"{ApiRoutes.Product.GetAll}?search={search}&page={page}&pageSize={pageSize}";
            var res = await client.GetAsync(url);

            var products = await JsonUtility.DeserializeWrappedListAsync<ProductDto>(res);
            return products;
        }

        public async Task<(List<ProductDto> products, int total)> GetODataFilteredAsync(
            string name, string barcode, int? categoryId,
            int? minPrice, int? maxPrice, int? minStock, int? maxStock,
            int page, int pageSize)
        
        {
            var client = CreateClient();
            var filters = new List<string>();

            if (!string.IsNullOrWhiteSpace(name))
                filters.Add($"contains(tolower(ProductName),'{name.ToLower()}')");
            if (!string.IsNullOrWhiteSpace(barcode))
                filters.Add($"contains(tolower(BarcodeValue),'{barcode.ToLower()}')");
            if (categoryId.HasValue)
                filters.Add($"CategoryId eq {categoryId}");
            if (minPrice.HasValue)
                filters.Add($"UnitPrice ge {minPrice}");
            if (maxPrice.HasValue)
                filters.Add($"UnitPrice le {maxPrice}");
            if (minStock.HasValue)
                filters.Add($"UnitsInStock ge {minStock}");
            if (maxStock.HasValue)
                filters.Add($"UnitsInStock le {maxStock}");

            var filterString = filters.Count > 0
                    ? $"$filter={Uri.EscapeDataString(string.Join(" and ", filters))}"
                    : "";
            var skip = (page - 1) * pageSize;
            var url = $"api/products/search-odata?{filterString}&$skip={skip}&$top={pageSize}&$count=true";

            Console.WriteLine("OData URL: " + url);
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Error content: " + errorContent);
                throw new Exception($"API Error: {response.StatusCode}");
            }

            response.EnsureSuccessStatusCode();

            var products = await JsonUtility.DeserializeODataResponseAsync<ProductDto>(response);

            return (products, products.Count);
        }


        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var client = CreateClient();
            var url = string.Format(ApiRoutes.Product.GetById, id);
            var res = await client.GetAsync(url);

            // Dùng ApiResponse<ProductDto> vì "data" là 1 object
            var product = await JsonUtility.DeserializeApiResponseAsyncObj<ProductDto>(res);
            return product;
        }


        public async Task<bool> CreateAsync(ProductUpdateDto product)
        {
            var client = CreateClient();
            var res = await client.PostAsJsonAsync(ApiRoutes.Product.Create, product);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, ProductUpdateDto dto)
        {
            var client = CreateClient();
            var url = string.Format(ApiRoutes.Product.Update, id);

            var res = await client.PutAsJsonAsync(url, dto); // ✅ không serialize thủ công

            if (!res.IsSuccessStatusCode)
            {
                var errorJson = await res.Content.ReadAsStringAsync();
                Console.WriteLine("❌ Error Response:");
                Console.WriteLine(errorJson); // <- xem nội dung lỗi
            }

            return res.IsSuccessStatusCode;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var client = CreateClient();
            var url = string.Format(ApiRoutes.Product.Delete, id);
            var res = await client.DeleteAsync(url);
            return res.IsSuccessStatusCode;
        }

        public async Task<List<ProductDto>> GetLowStockAsync()
        {
            var client = CreateClient();
            var res = await client.GetAsync(ApiRoutes.Product.LowStock);

            var products = await JsonUtility.DeserializeWrappedListAsync<ProductDto>(res);
            return products;
        }

        public async Task<ProductDto?> ScanProductAsync(string barcode)
        {
            var client = CreateClient();
            var url = string.Format(ApiRoutes.Product.Scan, barcode);
            var res = await client.GetAsync(url);

            var apiResponse = await JsonUtility.DeserializeApiResponseAsync<ProductDto>(res);
            return apiResponse.Data;
        }

        public async Task<List<ProductDto>> GetBySupplierIdAsync(int supplierId)
        {
            var client = CreateClient();
            var url = string.Format(ApiRoutes.Product.GetBySupplierId, supplierId);
            var res = await client.GetAsync(url);

            var products = await JsonUtility.DeserializeWrappedListAsync<ProductDto>(res);
            return products;
        }

        public async Task<List<ProductDto>> GetByCategoryIdAsync(int supplierId)
        {
            var client = CreateClient();
            var url = string.Format(ApiRoutes.Product.GetBySupplierId, supplierId);
            var res = await client.GetAsync(url);

            var products = await JsonUtility.DeserializeWrappedListAsync<ProductDto>(res);
            return products;
        }

        internal async Task<(string? products, double total)> GetFilteredAsync(string search, int? minPrice, int? maxPrice, int? minStock, int? maxStock, int page, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
    public class ODataResponse<T>
    {
        [JsonPropertyName("value")]
        public List<T> Value { get; set; }

        [JsonPropertyName("@odata.count")]
        public int? Count { get; set; }
    }

    public class ProductListDataWrapper<T>
    {
        public List<T> Data { get; set; }
        public int Total { get; set; }
    }

}
