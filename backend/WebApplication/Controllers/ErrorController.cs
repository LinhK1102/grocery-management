using Microsoft.AspNetCore.Mvc;

namespace GroceryWebApp.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)] // ✅ Ẩn controller này khỏi Swagger
    public class ErrorController : Controller
    {
        [HttpGet]
        [Route("Error/404")]
        public IActionResult Error404()
        {
            return View("NotFound");
        }
    }
}
