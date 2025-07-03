using BusinessObjects.Entities;
using DataAccess.DAO;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly WarehouseDAO _dao;
        public WarehouseRepository(ApplicationDbContext ctx) => _dao = new WarehouseDAO(ctx);

        public List<Warehouse> GetAllWarehouses() => _dao.GetAllWarehouses();
        public Warehouse GetWarehouseById(int id) => _dao.GetWarehouseById(id);
        public Warehouse CreateWarehouse(Warehouse w)
        {
            var existingWarehouse = _dao.GetWarehouseByName(w.WarehouseName);
            if (existingWarehouse != null)
            {
                throw new Exception($"Warehouse with name '{w.WarehouseName}' already exists.");
            }

            // Check if the warehouse name is "Undefinded" and handle it accordingly
            existingWarehouse = _dao.GetWarehouseByName("Undefinded");
            if (w.WarehouseId == 0 && existingWarehouse == null)
            {
                return _dao.CreateWarehouse(w);
            }

            // If the warehouse name is not "Undefinded", create a new warehouse
            else if (existingWarehouse != null)
                return _dao.CreateWarehouse(new Warehouse { WarehouseName = "Undefinded" });
            return null;
        }
        public Warehouse UpdateWarehouse(Warehouse w)
        {
            return _dao.UpdateWarehouse(w);
        }
        public bool DeleteWarehouse(int id) => _dao.DeleteWarehouse(id);

        public Warehouse GetWarehouseByName(string name)
        {
            return _dao.GetWarehouseByName(name);
        }

        public List<Warehouse> SearchWarehouses(string term)
        {
            return _dao.SearchWarehouses(term);
        }
        public List<Warehouse> GetWarehousesByProductId(int productId)
        {
            return _dao.GetWarehousesByProductId(productId);
        }
        public ProductWarehouse CreateProductWarehouse(Product product, Warehouse warehouse)
        {
            if(product == null || warehouse == null)
            {
                throw new ArgumentNullException("Product or Warehouse cannot be null.");
            }
            var existingProductWarehouse = _dao.GetProductWarehouse(product, warehouse);
            if (existingProductWarehouse != null)
            {
                throw new Exception($"Product {product.ProductName} already exists in warehouse {warehouse.WarehouseName}.");
            }
            var existingWarehouse = _dao.GetWarehouseById(warehouse.WarehouseId);
            if (warehouse.WarehouseId == 0 && existingWarehouse != null)
            {
                warehouse = CreateWarehouse(warehouse);
            }
            return _dao.CreateProductWarehouse(product, warehouse);
        }

        public List<Warehouse> GetWarehousesByItemId(string itemId)
        {
            return _dao.GetWarehousesByItemId(itemId);
        }

        public ProductWarehouse GetProductWarehouse(Product product, Warehouse warehouse)
        {
            return _dao.GetProductWarehouse(product, warehouse);
        }

        public ProductWarehouse UpdateProductWarehouse(Product product, Warehouse warehouse, int quantity)
        {
           return _dao.UpdateProductWarehouse(product, warehouse, quantity);
        }

        public bool DeleteProductWarehouse(Product product, Warehouse warehouse)
        {
           return _dao.DeleteProductWarehouse(product, warehouse);
        }

        public List<Product> GetProductsInWarehouse(int warehouseId)
        {
            return _dao.GetProductsInWarehouse(warehouseId);
        }
    }
}
