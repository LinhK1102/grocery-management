using BusinessObjects.DTOs;
using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

        public async Task<List<Product>> GetAllProductAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public Product GetProductById(int id) =>
            _context.Products
            .Include(c => c.Category)
            .Include(od => od.OrderDetails)
            .Include(i => i.Items)
            .FirstOrDefault(p => p.ProductId == id);
          public Product GetProductByName(string productName) =>
            _context.Products
            .Include(c => c.Category)
            .Include(od => od.OrderDetails)
            .Include(i => i.Items)
            .FirstOrDefault(p => p.ProductName == productName);

        public async Task<Product?> AddProductAsync(Product product)
        {
            try
            {
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
                return product;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding product: {ex.Message}");
                return null;
            }
        }


        public Product UpdateProduct(ProductUpdateDto updatedProduct)
        {
            try
            {
                var existingProduct = _context.Products.FirstOrDefault(p => p.ProductId == updatedProduct.ProductId);
                if (existingProduct == null) return null;

                // ⚠️ Chỉ cập nhật các field đơn giản
                existingProduct.ProductName = updatedProduct.ProductName;
                existingProduct.CategoryId = updatedProduct.CategoryId;
                existingProduct.SupplierId = updatedProduct.SupplierId;
                existingProduct.UnitsInStock = updatedProduct.UnitsInStock;
                existingProduct.UnitPrice = updatedProduct.UnitPrice;
                existingProduct.BarcodeValue = updatedProduct.BarcodeValue;
                existingProduct.ExpiryDuration = updatedProduct.ExpiryDuration;

                _context.Products.Update(existingProduct);
                _context.SaveChanges();

                return existingProduct;
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

        public bool AdjustStock(string barcode, int action, int quantity)
        {
            var product = _context.Products.FirstOrDefault(p => p.BarcodeValue == barcode);
            if (product == null) return false;

            if (action == 0) 
                product.UnitsInStock -= quantity; //sell action
            else if (action == 1)
                product.UnitsInStock += quantity; //restock action

            _context.SaveChanges();
            return true;
        }
        public List<Product> GetSupplierProductList(int supplierId)
        {
            return _context.Products
                .Where(p => p.SupplierId == supplierId)
                .ToList();
        }

        public List<Product> GetCategoryProductList(int categoryId)
        {
            return _context.Products
                .Where(p => p.CategoryId == categoryId)
                .ToList();
        }

        public IQueryable<Product> GetQueryable()
        {
            return _context.Products.AsNoTracking();
        }
    }
}
