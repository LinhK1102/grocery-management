using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
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
