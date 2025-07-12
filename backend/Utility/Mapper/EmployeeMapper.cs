using BusinessObjects.DTOs;
using BusinessObjects.Entities;
using Repositories.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Mapper
{
    public static class EmployeeMapper
    {
        public static Employee ToEmployeeEntity(this EmployeeRegisterRequest request, int retailOutletId)
        {
            return new Employee
            {
                EmployeeName = request.EmployeeName,
                EmployeeEmail = request.Email,
                EmployeeEmailTokenPass = request.Password, 
                RetailOutletId = retailOutletId
            };
        }

        public static EmployeeRegisterResponse ToRegisterResponse(this Employee employee, string token, string role)
        {
            return new EmployeeRegisterResponse
            {
                FullName = employee.EmployeeName,
                Email = employee.EmployeeEmail,
                Password = employee.EmployeeEmailTokenPass,
                Role = role,
                Token = token
            };
        }

        public static EmployeeLoginResponse ToLoginResponse(this Employee employee, string token, string role)
        {
            return new EmployeeLoginResponse
            {
                EmployeeId = employee.EmployeeId,
                FullName = employee.EmployeeName,
                Role = role,
                Token = token
            };
        }
    }
}
