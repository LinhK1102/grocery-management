using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace GroceryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public IActionResult GetAllCustomers() => Ok(_customerRepository.GetAllCustomers());

        [HttpGet("GetCustomerById/{id}")]
        public IActionResult GetCustomerById(int id) => Ok(_customerRepository.GetCustomerById(id));

        [HttpPost("CreateCustomer/{id}")]
        public IActionResult CreateCustomer([FromBody] Customer customer)
        {
            _customerRepository.CreateCustomer(customer);
            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.CustomerId }, customer);
        }

        [HttpPut("UpdateCustomer/{id}")]
        public IActionResult UpdateCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerId) return BadRequest();
            _customerRepository.UpdateCustomer(customer);
            return NoContent();
        }

        [HttpDelete("DeleteCustomer/{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            _customerRepository.DeleteCustomer(id);
            return NoContent();
        }

        [HttpGet("search")]
        public IActionResult SearchCustomers(string searchTerm) => Ok(_customerRepository.SearchCustomers(searchTerm));

        [HttpGet("high-discount")]
        public IActionResult GetHighDiscountCustomers([FromQuery] decimal minDiscount) => Ok(_customerRepository.GetHighDiscountCustomers(minDiscount));

        [HttpGet("frequent-buyers")]
        //public IActionResult GetCustomersByPurchaseFrequency([FromQuery] int minFrequency) => Ok(_customerRepository.GetCustomersByPurchaseFrequency(minFrequency));

        [HttpPost("update-discount")]
        public IActionResult UpdateCustomerDiscountRateWithLimit([FromQuery] int customerId, [FromQuery] decimal newRate, [FromQuery] decimal maxLimit)
        {
            bool updated = _customerRepository.UpdateCustomerDiscountRateWithLimit(customerId, newRate, maxLimit);
            return updated ? Ok() : BadRequest("Discount rate exceeds allowed limit");
        }
    }

}
