using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class SupplierDAO
    {
        private readonly ApplicationDbContext _context;
        public SupplierDAO(ApplicationDbContext context) => _context = context;

        public List<Supplier> GetAllSuppliers() => _context.Suppliers.ToList();
        public Supplier GetSupplierById(int id) => _context.Suppliers.Find(id);
        public void CreateSupplier(Supplier s) { _context.Suppliers.Add(s); _context.SaveChanges(); }
        public void UpdateSupplier(Supplier s) { _context.Suppliers.Update(s); _context.SaveChanges(); }
        public void DeleteSupplier(int id)
        {
            var hasProducts = _context.Products.Any(p => p.SupplierId == id);
            if (!hasProducts)
            {
                var s = _context.Suppliers.Find(id);
                if (s != null) { _context.Suppliers.Remove(s); _context.SaveChanges(); }
            }
        }

        public bool DeleteSupplierWithDependencyCheck(int supplierId)
        {
            var hasProducts = _context.Products.Any(p => p.SupplierId == supplierId);
            if (hasProducts)
                return false;

            var supplier = _context.Suppliers.Find(supplierId);
            if (supplier == null)
                return false;

            _context.Suppliers.Remove(supplier);
            _context.SaveChanges();
            return true;
        }

       
    }
}
