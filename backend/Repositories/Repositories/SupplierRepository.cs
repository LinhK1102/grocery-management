using BusinessObjects.Entities;
using DataAccess.DAO;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly SupplierDAO _dao;
        public SupplierRepository(ApplicationDbContext ctx) => _dao = new SupplierDAO(ctx);

        public List<Supplier> GetAllSuppliers() => _dao.GetAllSuppliers();
        public Supplier GetSupplierById(int id) => _dao.GetSupplierById(id);
        public void CreateSupplier(Supplier s) => _dao.CreateSupplier(s);
        public void UpdateSupplier(Supplier s) => _dao.UpdateSupplier(s);
        public void DeleteSupplier(int id) => _dao.DeleteSupplier(id);

        public bool DeleteSupplierWithDependencyCheck(int supplierId)
        {
            return _dao.DeleteSupplierWithDependencyCheck(supplierId);
        }
    }
}
