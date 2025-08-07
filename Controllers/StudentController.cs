using Microsoft.AspNetCore.Mvc;
using WebUseASP_test_.Models;

namespace WebUseASP_test_.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()

        {
            // Trong StudentController
            ViewData["PageIcon"] = "fa-users";
            ViewData["PageTitle"] = "Quản lý học sinh";
            return View();
        }
    }
}

