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

        public List<Warehouse> GetAllWarehouses() => _context.Warehouses.ToList();
        public Warehouse GetWarehouseById(int id) => _context.Warehouses.Find(id);
        public void CreateWarehouse(Warehouse w) { _context.Warehouses.Add(w); _context.SaveChanges(); }
        public void UpdateWarehouse(Warehouse w) { _context.Warehouses.Update(w); _context.SaveChanges(); }
        public void DeleteWarehouse(int id)
        {
            var w = _context.Warehouses.Find(id);
            if (w != null) { _context.Warehouses.Remove(w); _context.SaveChanges(); }
        }
    }
}
