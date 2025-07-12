using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IRetailOutletRepository
    {
        List<RetailOutlet> GetAllRetailOutlets();
        RetailOutlet GetRetailOutletById(int id);
        RetailOutlet CreateRetailOutlet(RetailOutlet ro);
        RetailOutlet UpdateRetailOutlet(RetailOutlet ro);
        void DeleteRetailOutlet(int id);
        List<Employee> GetEmployeesByOutlet(int outletId);
        Task EnsureDefaultRetailOutletAsync();
        RetailOutlet GetRetailOutletByName(string name);

    }
}
