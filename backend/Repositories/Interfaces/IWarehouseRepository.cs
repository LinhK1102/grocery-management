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
        List<Warehouse> GetAllWarehouses();
        Warehouse GetWarehouseById(int id);
        void CreateWarehouse(Warehouse w);
        void UpdateWarehouse(Warehouse w);
        void DeleteWarehouse(int id);
    }

}
