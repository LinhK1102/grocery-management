using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [Route("[controller]")]
    public class BarcodeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
