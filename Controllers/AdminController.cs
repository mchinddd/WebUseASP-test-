using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebUseASP_test_.Data;
using WebUseASP_test_.Models;

namespace WebUseASP_test_.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Trang tổng quan
        public IActionResult Dashboard()
        {
            // Thống kê nhanh
            ViewBag.StudentCount = _context.Students.Count();
            ViewBag.TeacherCount = _context.Users.Count(u => u.RoleID == 2);
            ViewBag.ClassCount = _context.Classes.Count();
            ViewBag.SubjectCount = _context.Subjects.Count();

            ViewData["PageIcon"] = "fa-home";
            ViewData["PageTitle"] = "Trang chủ Admin";

            return View();
        }

        // Quản lý người dùng
        public IActionResult Users()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        // Tạo người dùng mới (GET)
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        // Tạo người dùng mới (POST)
        [HttpPost]
        public IActionResult CreateUser(User model)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Users");
            }
            return View(model);
        }

        // Sửa người dùng (GET)
        [HttpGet]
        public IActionResult EditUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserID == id);
            if (user == null) return NotFound();
            return View(user);
        }

        // Sửa người dùng (POST)
        [HttpPost]
        public IActionResult EditUser(User model)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Update(model);
                _context.SaveChanges();
                return RedirectToAction("Users");
            }
            return View(model);
        }

        // Xóa người dùng
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserID == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("Users");
        }

        //hien thi du lieu student
        public IActionResult Students()
        {
            var students = _context.Students
                .Join(_context.Users,
                    student => student.UserID,
                    user => user.UserID,
                    (student, user) => new
                    {
                        student.StudentID,
                        student.StudentCode,
                        FullName = user.FullName,
                        student.DateOfBirth,
                        student.Gender,
                        student.Address,
                        ClassName = student.Class.ClassName
                    })
                .ToList();

            return View(students);
        }

        //hien thi du lieu teacher
        public IActionResult Teachers()
        {
            var teachers = _context.Teachers
                .Include(t => t.User)
                .Select(t => new
                {
                    t.TeacherID,
                    t.TeacherCode,
                    FullName = t.User.FullName,
                    Email = t.User.Email,
                    Phone = t.User.Phone,
                    t.Department,
                    t.Specialization
                })
                .ToList();

            return View(teachers);
        }


        //public IActionResult UsersByRole(int roleId)
        //{
        //    var users = _context.Users
        //        .Where(u => u.RoleID == roleId)
        //        .ToList();

        //    ViewBag.RoleID = roleId;
        //    ViewBag.RoleName = _context.Roles
        //        .Where(r => r.RoleID == roleId)
        //        .Select(r => r.RoleName)
        //        .FirstOrDefault();

        //    return View("Users", users); // dùng lại view Users.cshtml
        //}

        [HttpGet]
        public IActionResult CreateUserByRole(int roleId)
        {
            var user = new User
            {
                RoleID = roleId,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            ViewBag.RoleID = roleId;
            return View("CreateUser", user); // dùng lại view CreateUser.cshtml
        }

        [HttpPost]
        public IActionResult CreateUserByRole(User model)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(model);
                _context.SaveChanges();
                return RedirectToAction("UsersByRole", new { roleId = model.RoleID });
            }
            ViewBag.RoleID = model.RoleID;
            return View("CreateUser", model);
        }


    }
}
