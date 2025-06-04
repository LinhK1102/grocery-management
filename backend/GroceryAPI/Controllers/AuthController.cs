using Microsoft.AspNetCore.Mvc;
using Repositories.DTOs;
using Repositories.Interfaces;
using Repositories.Repositories;

namespace GroceryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;

        public AuthController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] EmployeeLoginRequest request)
        {

            var result = await _employeeRepository.LoginAsync(request);
            if (result == null)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(EmployeeRegisterRequest request)
        {
            var result = await _employeeRepository.RegisterAsync(request);
            
            return Ok(result);
        }

    }
}
