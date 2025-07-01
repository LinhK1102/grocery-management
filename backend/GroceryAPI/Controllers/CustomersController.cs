using BusinessObjects.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Utility.Common;

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

        [HttpGet("search")]
        public IActionResult SearchCustomers(string searchTerm)
        {
            var result = _customerRepository.SearchCustomers(searchTerm);
            return Ok(SystemStatus.Success(result, "Customer search completed."));
        }

        [HttpGet("high-discount")]
        public IActionResult GetHighDiscountCustomers([FromQuery] decimal minDiscount)
        {
            var result = _customerRepository.GetHighDiscountCustomers(minDiscount);
            return Ok(SystemStatus.Success(result, $"Customers with discount >= {minDiscount}"));
        }

        //[HttpGet("frequent-buyers")]
        //public IActionResult GetFrequentBuyers([FromQuery] int minFrequency)
        //{
        //    var result = _customerRepository.Get(minFrequency);
        //    return Ok(SystemStatus.Success(result, $"Customers with frequency >= {minFrequency}"));
        //}

        [HttpPost("update-discount")]
        public IActionResult UpdateCustomerDiscountRateWithLimit([FromQuery] int customerId, [FromQuery] decimal newRate, [FromQuery] decimal maxLimit)
        {
            bool updated = _customerRepository.UpdateCustomerDiscountRateWithLimit(customerId, newRate, maxLimit);
            return updated
                ? Ok(SystemStatus.Success($"Customer {customerId} discount updated successfully."))
                : BadRequest(SystemStatus.Fail("Discount rate exceeds allowed limit"));
        }
    }

}
