using BusinessObjects;
using System.Collections.Generic;

namespace Repositories.Interfaces
{
    public interface IProductRepository
    {
        List<Product> GetAllProduct();
        Product GetProductById(int id);
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(int id);

        Product GetProductByBarcode(string barcode);
        List<Product> GetLowStockProducts(int threshold);
        void AdjustStock(string barcode, string action, int quantity);

        List<Product> GetSupplierProductList(int supplierId);
    }
}
