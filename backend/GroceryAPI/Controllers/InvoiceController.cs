using BusinessObjects.Entities;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Utility.Common;

namespace GroceryAPI.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceController(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _invoiceRepository.GetAllAsync();
            return Ok(SystemStatus.Success(result, "Invoices retrieved successfully."));
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var invoice = await _invoiceRepository.GetWithDetailsAsync(new Invoice { InvoiceId = id });
            if (invoice == null)
                return NotFound(SystemStatus.Fail($"Invoice with ID {id} not found."));

            return Ok(SystemStatus.Success(invoice, "Invoice retrieved successfully."));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Invoice invoice)
        {
            var response = await _invoiceRepository.CreateAsync(invoice);
            if (response != null)
                return BadRequest(SystemStatus.Fail(response.Message));

            return Ok(SystemStatus.Success(response.Data, response.Message));
        }

    }
}
