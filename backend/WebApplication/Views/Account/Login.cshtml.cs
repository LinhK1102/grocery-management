using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication.Views.Account
{
    public class LoginModel : PageModel
    {
        public void OnGet()
        {
        }
    }
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

}
