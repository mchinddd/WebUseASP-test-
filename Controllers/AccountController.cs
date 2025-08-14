// Controllers/AccountController.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace WebUseASP_test_.Controllers
{
    public class AccountController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Xóa cookie xác thực
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Xóa session
            HttpContext.Session.Clear();

            // Chuyển hướng đến trang đăng nhập
            return RedirectToAction("Login", "Index");
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Trang đăng nhập (bạn cần tạo view này)
            return View();
        }
    }
}