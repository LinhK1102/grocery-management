using BusinessObjects;
using DataAccess.DAO;
using Repositories.DTOs;
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
        private readonly IJwtTokenGenerator _tokenGenerator;
        public EmployeeRepository(EmployeeDAO dao, IJwtTokenGenerator tokenGenerator)
        {
            _dao = dao;
            _tokenGenerator = tokenGenerator; // <- dòng này đảm bảo biến tồn tại
        }

        public List<Employee> GetAllEmployees() => _dao.GetAllEmployees();
        public Employee GetEmployeeById(int id) => _dao.GetEmployeeById(id);
        public void CreateEmployee(Employee e) => _dao.CreateEmployee(e);
        public void UpdateEmployee(Employee e) => _dao.UpdateEmployee(e);
        public void DeleteEmployee(int id) => _dao.DeleteEmployee(id);
        public List<Employee> GetTopSellingEmployees() => _dao.GetTopSellingEmployees();
        public async Task<EmployeeLoginResponse?> LoginAsync(EmployeeLoginRequest request)
        {
            var employee = await _dao.GetByEmailAsync(request.Email);
            if (employee == null) return null;

            // Password check (you may use BCrypt or similar)
            if (!BCrypt.Net.BCrypt.Verify(request.Password, employee.EmployeeEmailTokenPass))
                return null;

            var token = _tokenGenerator.GenerateToken(employee); // Implement separately

            return new EmployeeLoginResponse
            {
                EmployeeId = employee.EmployeeId,
                FullName = employee.EmployeeName,
                Role = "Employee Role",
                Token = token
            };
        }
    }
}
