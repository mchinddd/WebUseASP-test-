// LoginController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using WebUseASP_test_.Data;
using WebUseASP_test_.Models;
using System.Linq;
using System.Threading.Tasks;

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
            // Kiểm tra nếu đã đăng nhập thì chuyển hướng
            if (User.Identity.IsAuthenticated)
            {
                return RedirectBasedOnRole(User.FindFirst(ClaimTypes.Role)?.Value);
            }

            // Truyền lại username nếu có từ lần đăng nhập trước
            ViewBag.Username = TempData["Username"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string username, string password, bool remember)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Tạo claims identity
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.RoleID.ToString())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Cấu hình thuộc tính xác thực
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = remember, // "Ghi nhớ đăng nhập"
                    AllowRefresh = true
                };

                // Đăng nhập
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    authProperties);

                // Chuyển hướng theo role
                return RedirectBasedOnRole(user.RoleID.ToString());
            }

            // Lưu lại username để hiển thị lại form
            TempData["Username"] = username;
            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng";
            return View();
        }

        private IActionResult RedirectBasedOnRole(string roleId)
        {
            return roleId switch
            {
                "1" => RedirectToAction("Dashboard", "Admin"),
                "2" => RedirectToAction("Index", "Teacher"),
                _ => RedirectToAction("Dashboard", "Student")
            };
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }
    }
}