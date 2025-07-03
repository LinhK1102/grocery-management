using Utility.Common;
using WebApplication.Constants;
using WebApplication.Models.Dto;
using WebApplication.Service;

namespace WebApplication.Services
{
    public class ProductApiService : ApiClientService
    {
        public ProductApiService(IHttpClientFactory factory, IHttpContextAccessor contextAccessor, IConfiguration config)
            : base(factory, contextAccessor, config) { }

        public async Task<List<ProductDto>> GetAllAsync()
        {
            var client = CreateClient();
            var url = ApiRoutes.Product.GetAll;
            var res = await client.GetAsync(url);

            var products = await JsonUtility.DeserializeWrappedListAsync<ProductDto>(res);
            return products;
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


        public async Task<bool> CreateAsync(ProductDto product)
        {
            var client = CreateClient();
            var res = await client.PostAsJsonAsync(ApiRoutes.Product.Create, product);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, ProductDto product)
        {
            var client = CreateClient();
            var url = string.Format(ApiRoutes.Product.Update, id);
            var res = await client.PutAsJsonAsync(url, product);
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
    }
}
