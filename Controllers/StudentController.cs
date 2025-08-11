using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebUseASP_test_.Data; // namespace DbContext
using WebUseASP_test_.Models;

namespace WebUseASP_test_.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Lấy danh sách học sinh kèm thông tin User và Class
            var students = _context.Students
                .Include(s => s.User)
                .Include(s => s.Class)
                .ToList();

            ViewData["PageIcon"] = "fa-users";
            ViewData["PageTitle"] = "Quản lý học sinh";

            return View(students);
        }
    }
}
