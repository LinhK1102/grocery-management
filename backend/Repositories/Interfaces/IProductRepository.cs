using BusinessObjects.DTOs;
using BusinessObjects.Entities;
using System.Collections.Generic;

namespace Repositories.Interfaces
{
    public interface IProductRepository : ISeedableRepository
    {
        Task<List<Product>> GetAllProduct();
        Product GetProductById(int id);
        Product GetProductByName(string productName);
        Task<Product> AddProduct(Product product);
        Product UpdateProduct(ProductUpdateDto product);
        bool DeleteProduct(int id);

        Product GetProductByBarcode(string barcode);
        //Product GetProductInfByBarcode(string barcode);
        List<Product> GetLowStockProducts(int threshold);
        bool AdjustStock(string barcode, int action, int quantity);
        bool AdjustStock(int productId, int action, int quantity);

        List<Product> GetSupplierProductList(int supplierId);
        List<Product> GetCategoryProductList(int categoryId);
        IQueryable<Product> GetQueryable();
    }
}
