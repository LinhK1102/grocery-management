using BusinessObjects.Commons;
using BusinessObjects.Entities;
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

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var employee = new Employee
            {
                EmployeeName = request.EmployeeName,
                EmployeeEmail = request.Email,
                EmployeeEmailTokenPass = hashedPassword
            };

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

            //var token = _tokenGenerator.GenerateToken(registeredEmployee); // Nếu có JWT

            return new ApiResponse<EmployeeRegisterResponse>
            {
                Success = true,
                Message = "Registration successful.",
                Data = new EmployeeRegisterResponse
                {
                    FullName = registeredEmployee.EmployeeName,
                    Email = registeredEmployee.EmployeeEmail,
                    Password = registeredEmployee.EmployeeEmailTokenPass,
                    Role = "Employee Role" // Hoặc lấy từ DB
                }
            };
        }


        public void UpdateEmployee(Employee e) => _dao.UpdateEmployee(e);
        public void DeleteEmployee(int id) => _dao.DeleteEmployee(id);
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
            //var token = _tokenGenerator.GenerateToken(employee);

            return new ApiResponse<EmployeeLoginResponse>
            {
                Success = true,
                Message = "Login successful",
                Data = new EmployeeLoginResponse
                {
                    EmployeeId = employee.EmployeeId,
                    FullName = employee.EmployeeName,
                    Token = "token blank",
                    Role = "Employee"
                }
            };
        }


    }
}
