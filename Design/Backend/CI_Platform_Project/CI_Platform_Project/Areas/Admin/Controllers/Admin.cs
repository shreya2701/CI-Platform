using Microsoft.AspNetCore.Mvc;

namespace CI_Platform_Project.Areas.Admin.Controllers
{
    public class Admin : Controller
    {
        [Area("Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
