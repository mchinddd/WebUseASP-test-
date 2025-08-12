using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebUseASP_test_.Models;

namespace WebUseASP_test_.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Trong HomeController
            ViewData["PageIcon"] = "fa-home";
            ViewData["PageTitle"] = "Trang chủ";
            return View();
        }
    }
}
