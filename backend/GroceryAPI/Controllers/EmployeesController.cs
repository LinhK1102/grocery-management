using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

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

        [HttpGet("GetAllEmployees")]
        public IActionResult GetAllEmployees() => Ok(_employeeRepository.GetAllEmployees());

        [HttpGet("GetEmployeeById/{id}")]
        public IActionResult GetEmployeeById(int id) => Ok(_employeeRepository.GetEmployeeById(id));

        [HttpPost("CreateEmployee")]
        public IActionResult CreateEmployee([FromBody] Employee e)
        {
            _employeeRepository.CreateEmployee(e);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = e.EmployeeId }, e);
        }

        [HttpPut("UpdateEmployee/{id}")]
        public IActionResult UpdateEmployee(int id, Employee e)
        {
            if (id != e.EmployeeId) return BadRequest();
            _employeeRepository.UpdateEmployee(e);
            return NoContent();
        }

        [HttpDelete("DeleteEmployee/{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            _employeeRepository.DeleteEmployee(id);
            return NoContent();
        }

        [HttpGet("top-sellers")]
        public IActionResult GetTopSellingEmployees() => Ok(_employeeRepository.GetTopSellingEmployees());
    }
}
