using BusinessObjects.Entities;
using DataAccess.DAO;
using Microsoft.EntityFrameworkCore;
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
        public RetailOutlet CreateRetailOutlet(RetailOutlet ro)
        {
            // Nếu có ID hợp lệ
            if (ro.RetailOutletId > 0)
            {
                var existing = _dao.GetRetailOutletById(ro.RetailOutletId);
                if (existing != null)
                    return existing;
            }

            // Nếu có tên cụ thể (khác null và khác "Unassigned")
            if (!string.IsNullOrWhiteSpace(ro.RetailOutletName) && ro.RetailOutletName != "Unassigned")
            {
                var existingName = _dao.GetRetailOutletByRetailOutletName(ro.RetailOutletName);
                if (existingName != null)
                    return existingName;

                // Tạo mới nếu chưa tồn tại tên
                return _dao.CreateRetailOutlet(ro);
            }

            // Nếu không có gì hợp lệ, gán "Unassigned"
            return AssignUnassignedRetailOutletId();
        }


        public RetailOutlet UpdateRetailOutlet(RetailOutlet ro) => _dao.UpdateRetailOutlet(ro);
        public void DeleteRetailOutlet(int id) => _dao.DeleteRetailOutlet(id);

        public List<Employee> GetEmployeesByOutlet(int outletId)
        {
            throw new NotImplementedException();
        }
        private RetailOutlet AssignUnassignedRetailOutletId()
        {
            var unassignedOutlet = _dao.GetRetailOutletByRetailOutletName("Unassigned");

            if (unassignedOutlet != null) return unassignedOutlet;

            // Tạo mới nếu chưa tồn tại
            var newUnassigned = _dao.CreateRetailOutlet(new RetailOutlet
            {
                RetailOutletName = "Unassigned",
                RetailOutletLocation = "Unknown"
            });

            return newUnassigned;
        }

    }

}
