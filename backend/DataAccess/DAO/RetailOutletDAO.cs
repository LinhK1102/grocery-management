using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
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
        public RetailOutlet GetRetailOutletByName(string name) => _context.RetailOutlets.FirstOrDefault(r => r.RetailOutletName == name);
        public RetailOutlet GetRetailOutletByRetailOutletName(string retailName)
        {
            return _context.RetailOutlets.FirstOrDefault(ro => ro.RetailOutletName == retailName);
        }
        public RetailOutlet CreateRetailOutlet(RetailOutlet ro)
        {
            _context.RetailOutlets.Add(ro);
            _context.SaveChanges();
            return ro;
        }
        public RetailOutlet UpdateRetailOutlet(RetailOutlet ro) 
        { 
            _context.RetailOutlets.Update(ro); 
            _context.SaveChanges();
            return ro;
        }
        public bool DeleteRetailOutlet(int id)
        {
            var ro = _context.RetailOutlets.Find(id);
            if (ro != null) { _context.RetailOutlets.Remove(ro); _context.SaveChanges(); return false; }
            return true;
        }
        public List<Employee> GetEmployeesByOutlet(int outletId)
        {
            return _context.Employees
                           .Where(e => e.RetailOutletId == outletId)
                           .ToList();
        }

    }
}
