using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class RetailOutletDAO
    {
        private readonly ApplicationDbContext _context;
        public RetailOutletDAO(ApplicationDbContext context) => _context = context;

        public List<RetailOutlet> GetAllRetailOutlets() => _context.RetailOutlets.ToList();
        public RetailOutlet GetRetailOutletById(int id) => _context.RetailOutlets.Find(id);
        public void CreateRetailOutlet(RetailOutlet ro) { _context.RetailOutlets.Add(ro); _context.SaveChanges(); }
        public void UpdateRetailOutlet(RetailOutlet ro) { _context.RetailOutlets.Update(ro); _context.SaveChanges(); }
        public void DeleteRetailOutlet(int id)
        {
            var ro = _context.RetailOutlets.Find(id);
            if (ro != null) { _context.RetailOutlets.Remove(ro); _context.SaveChanges(); }
        }
    }
}
