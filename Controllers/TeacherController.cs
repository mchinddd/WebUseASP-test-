using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebUseASP_test_.Data; // namespace DbContext
using WebUseASP_test_.Models;
using System.Linq;

namespace WebUseASP_test_.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TeacherController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var students = _context.Students
           .Include(s => s.User)
           .Include(s => s.Class)
           .ToList();

            ViewData["PageIcon"] = "fa-home";
            ViewData["PageTitle"] = "Trang chủ Teacher";
            return View(students);
        }
    }
}
