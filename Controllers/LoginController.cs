// LoginController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using WebUseASP_test_.Data;
using WebUseASP_test_.Models;
using WebUseASP_test_.Helpers;
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
                // Convert RoleID -> RoleName
                string roleName = RoleHelper.GetRoleName(user.RoleID.ToString());

                // Claims
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, roleName) // <-- dùng RoleName
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = remember,
                    AllowRefresh = true
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    authProperties);

                return RedirectBasedOnRole(roleName);
            }

            TempData["Username"] = username;
            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng";
            return View();
        }

        private IActionResult RedirectBasedOnRole(string roleName)
        {
            return roleName switch
            {
                RoleHelper.Admin => RedirectToAction("Dashboard", "Admin"),
                RoleHelper.Teacher => RedirectToAction("Index", "Teacher"),
                RoleHelper.Student => RedirectToAction("Dashboard", "Student"),
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