using BusinessObjects.Commons;
using BusinessObjects.DTOs;
using BusinessObjects.Entities;
using DataAccess.DAO;
using Repositories.DTOs;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;
using Utility.Mapper;

namespace Repositories.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmployeeDAO _dao;
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly IRetailOutletRepository _retailOutletRepo;
        public EmployeeRepository(EmployeeDAO dao, IJwtTokenGenerator tokenGenerator, IRetailOutletRepository retailOutletRepo)
        {
            _dao = dao;
            _tokenGenerator = tokenGenerator; 
            _retailOutletRepo = retailOutletRepo;
        }

        public List<Employee> GetAllEmployees() => _dao.GetAllEmployees();
        public Employee GetEmployeeById(int id) => _dao.GetEmployeeById(id);
        public Employee CreateEmployee(Employee e) => _dao.CreateEmployee(e);
        public async Task<ApiResponse<EmployeeRegisterResponse>> RegisterAsync(EmployeeRegisterRequest request)
        {
            var existing = await _dao.GetByEmailAsync(request.Email);
            if (existing != null)
            {
                return new ApiResponse<EmployeeRegisterResponse>
                {
                    Success = false,
                    Message = "Email already exists."
                };
            }

            request.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

            //unassign retail outlet
            var retailOutlet = _retailOutletRepo.CreateRetailOutlet(new RetailOutlet());

            var employee = EmployeeMapper.ToEmployeeEntity(request, retailOutlet.RetailOutletId);

            await _dao.AddAsync(employee);

            // Lấy lại thông tin để tạo token
            var registeredEmployee = await _dao.GetByEmailAsync(request.Email);
            if (registeredEmployee == null)
            {
                return new ApiResponse<EmployeeRegisterResponse>
                {
                    Success = false,
                    Message = "Registration failed. Please try again."
                };
            }

            var token = _tokenGenerator.GenerateToken(registeredEmployee); // Nếu có JWT

            return new ApiResponse<EmployeeRegisterResponse>
            {
                Success = true,
                Message = "Registration successful.",
                Data = new EmployeeRegisterResponse
                {
                    FullName = registeredEmployee.EmployeeName,
                    Email = registeredEmployee.EmployeeEmail,
                    Password = registeredEmployee.EmployeeEmailTokenPass,
                    Role = "Employee Role", // Hoặc lấy từ DB
                    Token = token
                }
            };
        }


        public Employee UpdateEmployee(Employee e) => _dao.UpdateEmployee(e);
        public bool DeleteEmployee(int id) => _dao.DeleteEmployee(id);
        public List<Employee> GetTopSellingEmployees() => _dao.GetTopSellingEmployees();
        public async Task<ApiResponse<EmployeeLoginResponse>> LoginAsync(EmployeeLoginRequest request)
        {
            var employee = await _dao.GetByEmailAsync(request.Email);
            if (employee == null)
            {
                return new ApiResponse<EmployeeLoginResponse>
                {
                    Success = false,
                    Message = "Email not found"
                };
            }

            var hashedPassword = employee.EmployeeEmailTokenPass;
            if (string.IsNullOrWhiteSpace(hashedPassword))
            {
                return new ApiResponse<EmployeeLoginResponse>
                {
                    Success = false,
                    Message = "No password set for this account"
                };
            }

            bool isValid;
            try
            {
                isValid = BCrypt.Net.BCrypt.Verify(request.Password, hashedPassword);
            }
            catch (Exception ex)
            {
                return new ApiResponse<EmployeeLoginResponse>
                {
                    Success = false,
                    Message = $"Error verifying password: {ex.Message}"
                };
            }

            if (!isValid)
            {
                return new ApiResponse<EmployeeLoginResponse>
                {
                    Success = false,
                    Message = "Invalid credentials"
                };
            }

            // Tạo token nếu có
            var token = _tokenGenerator.GenerateToken(employee);

            return new ApiResponse<EmployeeLoginResponse>
            {
                Success = true,
                Message = "Login successful",
                Data = new EmployeeLoginResponse
                {
                    EmployeeId = employee.EmployeeId,
                    FullName = employee.EmployeeName,
                    Token = token,
                    Role = "Employee"
                }
            };
        }

        public async Task<Employee?> GetEmployeeByEmployeeName(string employeeName)
        {
            return await _dao.GetByNameAsync(employeeName);
        }

        public async Task<Employee?> GetEmployeeByEmail(string email)
        {
            return await _dao.GetEmployeeByEmail(email);
        }

        public async Task EnsureDefaultEmployeeAsync()
        {
            if (GetEmployeeByEmployeeName(UtitlityConstant.Undefined) == null)
            {
                CreateEmployee(new Employee
                {
                    EmployeeName = UtitlityConstant.Undefined,
                    EmployeeEmail = "unknown@employee.com",
                    EmployeeEmailTokenPass = "N/A",
                    RetailOutletId = _retailOutletRepo.GetRetailOutletByName(UtitlityConstant.Undefined)?.RetailOutletId ?? 0
                });
            }
        }


        //public async List<Employee> GetEmployeeBySearchTerm()
        //{
        //    return await _dao.
        //}
    }
}
