using Microsoft.AspNetCore.Mvc;

namespace Unique.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
