using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class EmployeeDAO
    {
        private readonly ApplicationDbContext _context;
        public EmployeeDAO(ApplicationDbContext context) => _context = context;

        public List<Employee> GetAllEmployees() => _context.Employees.ToList();
        public Employee GetEmployeeById(int id) => _context.Employees.Find(id);
        public void CreateEmployee(Employee e) { _context.Employees.Add(e); _context.SaveChanges(); }
        public void UpdateEmployee(Employee e) { _context.Employees.Update(e); _context.SaveChanges(); }
        public void DeleteEmployee(int id)
        {
            var e = _context.Employees.Find(id);
            if (e != null) { _context.Employees.Remove(e); _context.SaveChanges(); }
        }

        public List<Employee> GetTopSellingEmployees() => _context.Employees
            .OrderByDescending(e => e.Orders.Count)
            .Take(5)
            .ToList();
    }
}
