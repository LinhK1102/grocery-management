using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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

        public Supplier? CreateSupplier(Supplier s)
        {
            try
            {
                if (s.Products == null)
                    s.Products = new List<Product>();

                _context.Suppliers.Add(s);
                _context.SaveChanges();
                return s;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating supplier: {ex.Message}");
                return null;
            }
        }

        public Supplier? UpdateSupplier(Supplier s)
        {
            try
            {
                var existing = _context.Suppliers.FirstOrDefault(x => x.SupplierId == s.SupplierId);
                if (existing == null) return null;

                // Update từng trường
                existing.SupplierName = s.SupplierName;
                existing.SupplierEmail = s.SupplierEmail;
                existing.SupplierPhoneNumber = s.SupplierPhoneNumber;

                _context.SaveChanges();
                return existing;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error update suplliers.");
            }
        }

        public bool DeleteSupplier(int id)
        {
            var hasProducts = _context.Products.Any(p => p.SupplierId == id);
            if (!hasProducts)
            {
                var s = _context.Suppliers.Find(id);
                if (s != null) { _context.Suppliers.Remove(s); _context.SaveChanges(); return true; }
            }
            else
            {
                throw new InvalidOperationException("Cannot delete supplier with existing products.");
            }
            return false;
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

        public List<Supplier> SearchSuppliers(string term)
        {
            return _context.Suppliers
                .Where(s => s.SupplierName.Contains(term) || s.SupplierEmail.Contains(term))
                .ToList();
        }

        public Supplier GetSupplierBySuplierName(string supplierName) 
            => _context.Suppliers.FirstOrDefault(s => s.SupplierName.Contains(supplierName));

        public int GetOrCreateUnknownSupplierId(Supplier supplier)
        {
            var existing = GetSupplierBySuplierName(supplier.SupplierName);
            if (existing != null) return existing.SupplierId;

            var unknown = CreateSupplier(
                new Supplier
                {
                    SupplierName = "Unknown Supplier",
                    SupplierEmail = "N/A",
                    SupplierPhoneNumber = "N/A"
                });
            if (unknown == null)
                throw new Exception("Failed to create 'Unknown Supplier'.");

            return unknown.SupplierId;
        }
    }
}
