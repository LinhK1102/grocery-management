using BusinessObjects.Entities;
using System.Collections.Generic;

namespace Repositories.Interfaces
{
    public interface IProductRepository
    {
        List<Product> GetAllProduct();
        Product GetProductById(int id);
        Product AddProduct(Product product);
        Product UpdateProduct(Product product);
        bool DeleteProduct(int id);

        Product GetProductByBarcode(string barcode);
        //Product GetProductInfByBarcode(string barcode);
        List<Product> GetLowStockProducts(int threshold);
        void AdjustStock(string barcode, string action, int quantity);

        List<Product> GetSupplierProductList(int supplierId);
    }
}
