using BusinessObjects.Entities;
using DataAccess.DAO;
using Repositories.Interfaces;
using System.Collections.Generic;

namespace Repositories.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDAO _dao;
        private readonly CategoryDAO _categoryDao;
        private readonly SupplierDAO _supplierDao;
        public ProductRepository(ApplicationDbContext context, CategoryDAO categoryDao, SupplierDAO supplierDao)
        {
            _dao = new ProductDAO(context);
            _categoryDao = categoryDao;
            _supplierDao = supplierDao;
        }

        public List<Product> GetAllProduct() => _dao.GetAllProduct();
        public Product GetProductById(int id) => _dao.GetProductById(id);
        public Product AddProduct(Product product)
        {
            if (product.CategoryId == 0)
            {
                product.CategoryId = _categoryDao.GetOrCreateUncategorizedCategoryId(product.Category);
            }

            if (product.SupplierId == 0)
            {
                product.SupplierId = _supplierDao.GetOrCreateUnknownSupplierId(product.Supplier);
            }
            var status = _dao.AddProduct(product);
            return product;
        }
        public Product UpdateProduct(Product product) => _dao.UpdateProduct(product);
        public bool DeleteProduct(int id) => _dao.DeleteProduct(id);

        public Product GetProductByBarcode(string barcode) => _dao.GetProductByBarcode(barcode);

        public List<Product> GetLowStockProducts(int threshold) => _dao.GetLowStockProducts(threshold);

        public void AdjustStock(string barcode, string action, int quantity) => _dao.AdjustStock(barcode, action, quantity);

        public List<Product> GetSupplierProductList(int supplierId) => _dao.GetSupplierProductList(supplierId);
    }
}
