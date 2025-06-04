using BusinessObjects.Entities;
using DataAccess.DAO;
using Repositories.Interfaces;
using System.Collections.Generic;

namespace Repositories.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDAO _dao;

        public ProductRepository(ApplicationDbContext context)
        {
            _dao = new ProductDAO(context);
        }

        public List<Product> GetAllProduct() => _dao.GetAllProduct();
        public Product GetProductById(int id) => _dao.GetProductById(id);
        public void AddProduct(Product product) => _dao.AddProduct(product);
        public void UpdateProduct(Product product) => _dao.UpdateProduct(product);
        public void DeleteProduct(int id) => _dao.DeleteProduct(id);

        public Product GetProductByBarcode(string barcode) => _dao.GetProductByBarcode(barcode);

        public List<Product> GetLowStockProducts(int threshold) => _dao.GetLowStockProducts(threshold);

        public void AdjustStock(string barcode, string action, int quantity) => _dao.AdjustStock(barcode, action, quantity);

        public List<Product> GetSupplierProductList(int supplierId) => _dao.GetSupplierProductList(supplierId);
    }
}
