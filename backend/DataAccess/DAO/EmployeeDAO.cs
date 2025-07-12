using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
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

        public List<Employee> GetAllEmployees() => _context.Employees.OrderBy(e => e.EmployeeId).ToList();
        public Employee GetEmployeeById(int id) => _context.Employees.Find(id);
        public Employee CreateEmployee(Employee e) { _context.Employees.Add(e); _context.SaveChanges(); return e; }
        public async Task<Employee> AddAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }
        public Employee UpdateEmployee(Employee e) 
        {
            try
            {
                var existing = _context.Employees.FirstOrDefault(emp => emp.EmployeeId == e.EmployeeId);

                if (existing == null)
                    return null;

                existing.EmployeeName = e.EmployeeName;
                existing.EmployeeEmail = e.EmployeeEmail;
                existing.RetailOutletId = e.RetailOutletId;

                _context.SaveChanges();
                return e;
            }
            catch (Exception exp)
            {
                throw new InvalidCastException($"Invalid exception: {exp}");
            }
        }
        public bool DeleteEmployee(int id)
        {
            var e = _context.Employees.Find(id);
            if (e != null) { _context.Employees.Remove(e); _context.SaveChanges(); return true; }
            return false;
        }

        public List<Employee> GetTopSellingEmployees() => _context.Employees
            .OrderByDescending(e => e.Orders.Count)
            .Take(5)
            .ToList();

        public async Task<Employee?> GetByEmailAsync(string email)
        => await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeEmail == email);
        public async Task<Employee?> GetByNameAsync(string name)
        => await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeName == name);
        public async Task<Employee?> GetEmployeeByEmail(string email)
            => await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeEmail == email);

    }
}
