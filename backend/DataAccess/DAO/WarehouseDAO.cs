using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class WarehouseDAO
    {
        private readonly ApplicationDbContext _context;
        public WarehouseDAO(ApplicationDbContext context) => _context = context;

        public List<Warehouse> GetAllWarehouses() => _context.Warehouses.OrderBy(w => w.WarehouseId).ToList();
        public Warehouse GetWarehouseById(int id) => _context.Warehouses.Find(id);
        public Warehouse CreateWarehouse(Warehouse w) { _context.Warehouses.Add(w); _context.SaveChanges(); return w; }
        public Warehouse UpdateWarehouse(Warehouse w) 
        {
            var existing = _context.Warehouses.FirstOrDefault(x => x.WarehouseId == w.WarehouseId);
            if (existing == null) return null;

            // Update từng trường
            existing.WarehouseName = w.WarehouseName;
            existing.WarehouseLocation = w.WarehouseLocation;

            _context.SaveChanges();
            return existing;
        }
        public bool DeleteWarehouse(int id)
        {
            var w = _context.Warehouses.Find(id);
            if (w != null) { _context.Warehouses.Remove(w); _context.SaveChanges(); return true; }
            return false;
        }
        public List<Warehouse> SearchWarehouses(string term)
        {
            return _context.Warehouses
                .Where(w => w.WarehouseName.Contains(term) || w.WarehouseLocation.Contains(term))
                .ToList();
        }
        public List<Warehouse> GetWarehousesByProductId(int productId)
        {
            return _context.Warehouses
                .Where(w => w.ProductWarehouses.Any(i => i.ProductId == productId))
                .ToList();
        }
        public ProductWarehouse CreateProductWarehouse(Product product, Warehouse warehouse)
        {
            var productWarehouse = new ProductWarehouse
            {
                ProductId = product.ProductId,
                WarehouseId = warehouse.WarehouseId,
            };
            _context.ProductWarehouse.Add(productWarehouse);
            _context.SaveChanges();
            return productWarehouse;
        }
        public ProductWarehouse GetProductWarehouse(Product product, Warehouse warehouse)
        {
            return _context.ProductWarehouse
                .FirstOrDefault(pw => pw.ProductId == product.ProductId && pw.WarehouseId == warehouse.WarehouseId);
        }
        public ProductWarehouse UpdateProductWarehouse(Product product, Warehouse warehouse, int quantity)
        {
            var productWarehouse = _context.ProductWarehouse
                .FirstOrDefault(pw => pw.ProductId == product.ProductId && pw.WarehouseId == warehouse.WarehouseId);
            if (productWarehouse != null)
            {
                productWarehouse.Quantity = quantity;
                _context.ProductWarehouse.Update(productWarehouse);
                _context.SaveChanges();
                return productWarehouse;
            }
            return null;
        }
        public bool DeleteProductWarehouse(Product product, Warehouse warehouse)
        {
            var productWarehouse = _context.ProductWarehouse
                .FirstOrDefault(pw => pw.ProductId == product.ProductId && pw.WarehouseId == warehouse.WarehouseId);
            if (productWarehouse != null)
            {
                _context.ProductWarehouse.Remove(productWarehouse);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public List<Product> GetProductsInWarehouse(int warehouseId)
        {
            return _context.ProductWarehouse
                .Where(pw => pw.WarehouseId == warehouseId)
                .Select(pw => pw.Product)
                .ToList();
        }
        public List<Warehouse> GetWarehousesByItemId(string itemId)
        {
            return _context.Warehouses
                .Where(w => w.ProductWarehouses.Any(pw => pw.Product.Items.Any(i => i.ItemId == itemId)))
                .ToList();
        }
        public Warehouse GetWarehouseByName(string name)
        {
            return _context.Warehouses
                  .FirstOrDefault(w => w.WarehouseName.ToUpper() == name.ToUpper());
        }
    }
}
