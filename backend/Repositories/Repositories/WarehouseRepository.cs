using BusinessObjects;
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
        public void CreateWarehouse(Warehouse w) => _dao.CreateWarehouse(w);
        public void UpdateWarehouse(Warehouse w) => _dao.UpdateWarehouse(w);
        public void DeleteWarehouse(int id) => _dao.DeleteWarehouse(id);
    }
}
