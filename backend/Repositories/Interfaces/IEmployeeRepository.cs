using BusinessObjects.Commons;
using BusinessObjects.Entities;
using Repositories.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        List<Employee> GetAllEmployees();
        Employee GetEmployeeById(int id);
        Task<ApiResponse<EmployeeLoginResponse>> LoginAsync(EmployeeLoginRequest request);
        Task<ApiResponse<EmployeeRegisterResponse>> RegisterAsync(EmployeeRegisterRequest request);
        void CreateEmployee(Employee e);
        void UpdateEmployee(Employee e);
        void DeleteEmployee(int id);
        List<Employee> GetTopSellingEmployees();
    }
}
