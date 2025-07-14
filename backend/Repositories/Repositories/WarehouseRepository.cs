using BusinessObjects.Entities;
using DataAccess.DAO;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;

namespace Repositories.Repositories
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly WarehouseDAO _dao;
        public WarehouseRepository(WarehouseDAO warehouseDAO) => _dao = warehouseDAO;

        public List<Warehouse> GetAllWarehouses() => _dao.GetAllWarehouses();
        public Warehouse GetWarehouseById(int id) => _dao.GetWarehouseById(id);
        public Warehouse CreateWarehouse(Warehouse w)
        {
            var existingWarehouse = _dao.GetWarehouseByName(w.WarehouseName);
            if (existingWarehouse != null)
            {
                return null;
            }

            // Check if the warehouse ID is 0 and the default "Undefinded" warehouse does  exist
            existingWarehouse = _dao.GetWarehouseByName(UtitlityConstant.Undefined);
            if (w.WarehouseId == 0 && existingWarehouse == null)
            {//create undifined
                return _dao.CreateWarehouse(new Warehouse { WarehouseName = UtitlityConstant.Undefined, WarehouseLocation = UtitlityConstant.Undefined});
            }

            return _dao.CreateWarehouse(w);
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
            if (product == null) //check product is sent
            {
                throw new ArgumentNullException("Product cannot be null.");
            }
            else if (product != null && warehouse != null) // check exist relationshop
            {
                var existingProductWarehouse = _dao.GetProductWarehouse(product, warehouse);
                if (existingProductWarehouse != null)
                    throw new Exception($"Product {product.ProductName} already exists in warehouse {warehouse.WarehouseName}.");
            }

            //create new Undefined warehouse if not exists
            var existingWarehouse = _dao.GetWarehouseByName(UtitlityConstant.Undefined);
            warehouse = (existingWarehouse != null)
                ? existingWarehouse
                : CreateWarehouse(warehouse);
            return _dao.CreateProductWarehouse(product, existingWarehouse);
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

        public async Task<bool> EnsureSeedDataAsync()
        {
            if (await GetUndefinedWarehouseIdAsync() == 0)
            {
                var defaultWarehouse = new Warehouse
                {
                    WarehouseName = UtitlityConstant.Warehouse_Default_Name,
                    WarehouseLocation = UtitlityConstant.Warehouse_Default_Location
                };
                return CreateWarehouse(defaultWarehouse) != null;
            }
            return true;
        }

        public async Task<int> GetUndefinedWarehouseIdAsync()
        {
            return (_dao.GetWarehouseByName(UtitlityConstant.Warehouse_Default_Name))?.WarehouseId ?? 0;
        }

    }
}
