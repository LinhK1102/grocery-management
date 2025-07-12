using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IWarehouseRepository
    {
        // Warehouse CRUD
        List<Warehouse> GetAllWarehouses();
        Warehouse GetWarehouseById(int id);
        Warehouse CreateWarehouse(Warehouse w);
        Warehouse UpdateWarehouse(Warehouse w);
        bool DeleteWarehouse(int id);

        // Search & Queries
        List<Warehouse> SearchWarehouses(string term);
        List<Warehouse> GetWarehousesByProductId(int productId);
        List<Warehouse> GetWarehousesByItemId(string itemId);
        Warehouse GetWarehouseByName(string name);

        // ProductWarehouse logic
        ProductWarehouse CreateProductWarehouse(Product product, Warehouse warehouse);
        ProductWarehouse GetProductWarehouse(Product product, Warehouse warehouse);
        ProductWarehouse UpdateProductWarehouse(Product product, Warehouse warehouse, int quantity);
        bool DeleteProductWarehouse(Product product, Warehouse warehouse);
        List<Product> GetProductsInWarehouse(int warehouseId);
        Task EnsureDefaultWarehouseAsync();
    }

}
