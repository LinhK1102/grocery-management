using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface ISupplierRepository
    {
        Task EnsureDefaultSupplierAsync();
        List<Supplier> GetAllSuppliers();
        Supplier GetSupplierById(int id);
        Supplier GetSupplierByName(string name);
        Supplier CreateSupplier(Supplier s);
        Supplier UpdateSupplier(Supplier s);
        void DeleteSupplier(int id);

        bool DeleteSupplierWithDependencyCheck(int supplierId);
    }
}
