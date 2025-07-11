using BusinessObjects.Entities;
using DataAccess.DAO;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;

namespace Repositories.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly SupplierDAO _dao;
        public SupplierRepository(ApplicationDbContext ctx) => _dao = new SupplierDAO(ctx);

        public List<Supplier> GetAllSuppliers() => _dao.GetAllSuppliers();
        public Supplier GetSupplierById(int id) => _dao.GetSupplierById(id);
        public Supplier CreateSupplier(Supplier s)
        {
            if (s.Products == null)
                s.Products = new List<Product>();

            return _dao.CreateSupplier(s);
        }
        public Supplier UpdateSupplier(Supplier s)
        {
            var existingProduct = _dao.GetSupplierById(s.SupplierId);
            //case update by api, no product related
            if (existingProduct != null)
                s.Products = existingProduct.Products;
            else if (existingProduct == null && s.Products == null) //completely new
                return null;

            return _dao.UpdateSupplier(s);
        }
        public void DeleteSupplier(int id) => _dao.DeleteSupplier(id);

        public bool DeleteSupplierWithDependencyCheck(int supplierId)
        {
            var existingProduct = _dao.GetSupplierBySuplierName(UtitlityConstant.Unknown);
            //case product related
            if (existingProduct != null && existingProduct.Products != null)
                return false;

            return _dao.DeleteSupplierWithDependencyCheck(supplierId);
        }
    }
}
