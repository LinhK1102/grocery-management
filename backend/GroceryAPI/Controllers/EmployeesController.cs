using BusinessObjects.DTOs;
using BusinessObjects.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repositories.DTOs;
using Repositories.Interfaces;
using System.Threading.Tasks;
using Utility.Common;
using GroceryWebApp.Models.Dto;
using Microsoft.AspNetCore.OData.Query;
using static GroceryWebApp.Constants.ApiRoutes;


namespace GroceryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeesController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        [HttpGet]
        public IActionResult GetAllEmployees()
        {
            var employees = _employeeRepository.GetAllEmployees();
            return Ok(SystemStatus.Success(employees, "Employee list retrieved."));
        }

        [HttpGet("{id}")]
        public IActionResult GetEmployeeById(int id)
        {
            var employee = _employeeRepository.GetEmployeeById(id);
            if (employee == null)
                return BadRequest(SystemStatus.Fail("Employee not found."));
            return Ok(SystemStatus.Success(employee, "Employee found."));
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeRegisterRequest e)
        {
            if (string.IsNullOrWhiteSpace(e.Email))
                return Ok(SystemStatus.Fail("Email is required."));

            var existingEmployee = await _employeeRepository.GetEmployeeByEmail(e.Email);
            if (existingEmployee != null)
                return Ok(SystemStatus.Fail($"Email {e.Email} already existed."));

            var createdEmployee = (await _employeeRepository.RegisterAsync(e)).Data;
            return CreatedAtAction(nameof(GetEmployeeByEmployeeeName), new { employeeeName = e.EmployeeName },
                SystemStatus.Success(createdEmployee, "Employee created successfully."));
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEmployee(int id, [FromBody] EmployeeDto dto)
        {
            if (id != dto.EmployeeId)
                return BadRequest(SystemStatus.Fail("Mismatched employee ID."));

            var employee = new BusinessObjects.Entities.Employee
            {
                EmployeeId = dto.EmployeeId,
                EmployeeName = dto.EmployeeName,
                EmployeeEmail = dto.EmployeeEmail,
                RetailOutletId = dto.RetailOutletId
            };

            _employeeRepository.UpdateEmployee(employee);

            if (employee == null)
                return Ok(SystemStatus.Fail("Employee not found."));

            return Ok(SystemStatus.Success(employee, "Employee updated successfully."));
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            _employeeRepository.DeleteEmployee(id);
            return Ok(SystemStatus.Success($"Employee {id} deleted successfully."));
        }

        [HttpGet("top-sellers")]
        public IActionResult GetTopSellingEmployees()
        {
            var topEmployees = _employeeRepository.GetTopSellingEmployees();
            return Ok(SystemStatus.Success(topEmployees, "Top selling employees retrieved."));
        }

        [EnableQuery]
        [HttpGet("search/")]
        public IActionResult Search()
        {
            var all = _employeeRepository.GetAllEmployees(); // return IQueryable<Employee>
            return Ok(all);
        }

        [HttpGet("search/{employeeeEmail}")]
        public async Task<IActionResult> GetEmployeeByEmployeeeName(string employeeeEmail)
        {
            if (employeeeEmail.IsNullOrEmpty()) return Ok(SystemStatus.Fail("Employees is null."));

            var employees = await _employeeRepository.GetEmployeeByEmail(employeeeEmail);

            if (employees == null)
                return Ok(SystemStatus.Fail("Employee not found."));

            return Ok(SystemStatus.Success(employees, "All employees retrieved."));
        }


    }
}
