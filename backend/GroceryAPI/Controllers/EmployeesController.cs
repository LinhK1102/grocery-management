using BusinessObjects.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Utility.Common;

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
        public IActionResult CreateEmployee([FromBody] Employee e)
        {
            _employeeRepository.CreateEmployee(e);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = e.EmployeeId },
                SystemStatus.Success(e, "Employee created successfully."));
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEmployee(int id, [FromBody] Employee e)
        {
            if (id != e.EmployeeId)
                return BadRequest(SystemStatus.Fail("Mismatched employee ID."));

            _employeeRepository.UpdateEmployee(e);
            return Ok(SystemStatus.Success(e, "Employee updated successfully."));
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
    }
}
