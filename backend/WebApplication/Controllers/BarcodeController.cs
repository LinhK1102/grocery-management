using Microsoft.AspNetCore.Mvc;

namespace GroceryWebApp.Controllers
{
    [Route("[controller]")]
    public class BarcodeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
