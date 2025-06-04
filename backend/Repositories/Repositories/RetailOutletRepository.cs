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
    public class RetailOutletRepository : IRetailOutletRepository
    {
        private readonly RetailOutletDAO _dao;
        public RetailOutletRepository(ApplicationDbContext ctx) => _dao = new RetailOutletDAO(ctx);

        public List<RetailOutlet> GetAllRetailOutlets() => _dao.GetAllRetailOutlets();
        public RetailOutlet GetRetailOutletById(int id) => _dao.GetRetailOutletById(id);
        public void CreateRetailOutlet(RetailOutlet ro) => _dao.CreateRetailOutlet(ro);
        public void UpdateRetailOutlet(RetailOutlet ro) => _dao.UpdateRetailOutlet(ro);
        public void DeleteRetailOutlet(int id) => _dao.DeleteRetailOutlet(id);
    }

}
