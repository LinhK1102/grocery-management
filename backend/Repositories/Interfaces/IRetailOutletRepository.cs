using BusinessObjects;
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
        void CreateRetailOutlet(RetailOutlet ro);
        void UpdateRetailOutlet(RetailOutlet ro);
        void DeleteRetailOutlet(int id);
    }
}
