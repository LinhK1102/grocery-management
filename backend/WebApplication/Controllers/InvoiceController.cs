using Microsoft.AspNetCore.Mvc;
using GroceryWebApp.Service;

namespace GroceryWebApp.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly InvoiceApiService _invoiceService;
        private readonly InvoiceItemApiService _invoiceItemService;

        public InvoiceController(InvoiceApiService invoiceService, InvoiceItemApiService invoiceItemService)
        {
            _invoiceService = invoiceService;
            _invoiceItemService = invoiceItemService;
        }

        public async Task<IActionResult> Index()
        {
            var invoices = await _invoiceService.GetAllAsync();
            return View(invoices);
        }

        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null) return View("NotFound");

            var items = await _invoiceItemService.GetByInvoiceIdAsync(id);
            return View(Tuple.Create(invoice, items));
        }
    }
}
