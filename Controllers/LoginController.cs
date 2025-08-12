using Microsoft.AspNetCore.Mvc;
using WebUseASP_test_.Data;
using WebUseASP_test_.Models;
using System.Linq;

namespace WebUseASP_test_.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                // Điều hướng theo role
                if (user.RoleID == 1)
                    return RedirectToAction("Dashboard", "Admin");
                else if (user.RoleID == 2)
                    return RedirectToAction("Index", "Teacher");
                else
                    return RedirectToAction("Index", "Student");
            }

            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng";
            return View();
        }
    }
}
