using BusinessObjects;
using DataAccess.DAO;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmployeeDAO _dao;
        public EmployeeRepository(ApplicationDbContext ctx) => _dao = new EmployeeDAO(ctx);

        public List<Employee> GetAllEmployees() => _dao.GetAllEmployees();
        public Employee GetEmployeeById(int id) => _dao.GetEmployeeById(id);
        public void CreateEmployee(Employee e) => _dao.CreateEmployee(e);
        public void UpdateEmployee(Employee e) => _dao.UpdateEmployee(e);
        public void DeleteEmployee(int id) => _dao.DeleteEmployee(id);
        public List<Employee> GetTopSellingEmployees() => _dao.GetTopSellingEmployees();
    }
}
