using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.DAO
{
    public class ProductDAO
    {
        private readonly ApplicationDbContext _context;

        public ProductDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Product> GetAllProduct() => _context.Products.Include(p => p.Category).ToList();

        public Product GetProductById(int id) =>
            _context.Products
            .Include(c => c.Category)
            .Include(od => od.OrderDetails)
            .Include(i => i.Items)
            .FirstOrDefault(p => p.ProductId == id);

        public Product? AddProduct(Product product)
        {
            try
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return product;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding product: {ex.Message}");
                return null;
            }
        }

        public Product UpdateProduct(Product product)
        {
            try
            {
                _context.Products.Update(product);
                _context.SaveChanges();
                return product;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Update product: {ex.Message}");
                return null;
            }
        }

        public bool DeleteProduct(int id)
        {
            try
            {
                var product = _context.Products.Find(id);
                if (product != null)
                {
                    _context.Products.Remove(product);
                    _context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Update product: {ex.Message}");
                return false;
            }
            return false;
        }

        public Product GetProductByBarcode(string barcode)
            => _context.Products.FirstOrDefault(p => p.BarcodeValue == barcode);


        public List<Product> GetLowStockProducts(int threshold)
            => _context.Products.Where(p => p.UnitsInStock < threshold).ToList();

        public void AdjustStock(string barcode, string action, int quantity)
        {
            var product = _context.Products.FirstOrDefault(p => p.BarcodeValue == barcode);
            if (product == null) return;

            if (action == "sell")
                product.UnitsInStock -= quantity;
            else if (action == "receive")
                product.UnitsInStock += quantity;

            _context.SaveChanges();
        }
        public List<Product> GetSupplierProductList(int supplierId)
        {
            return _context.Products
                .Where(p => p.SupplierId == supplierId)
                .ToList();
        }
    }
}
