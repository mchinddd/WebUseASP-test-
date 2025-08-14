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

        // Tạo người dùng mới (POST) - ĐÃ CẬP NHẬT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUser(User model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra username đã tồn tại
                    if (_context.Users.Any(u => u.Username == model.Username))
                    {
                        ModelState.AddModelError("Username", "Tài khoản đã tồn tại");
                        return View(model);
                    }

                    // Mã hóa mật khẩu
                    model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
                    model.CreatedDate = DateTime.Now;

                    _context.Users.Add(model);
                    _context.SaveChanges();

                    // Tạo Student/Teacher nếu cần
                    if (model.RoleID == 3) // Student
                    {
                        _context.Students.Add(new Student
                        {
                            UserID = model.UserID,
                            StudentCode = "ST" + model.UserID.ToString("D4"),
                            DateOfBirth = DateTime.Today.AddYears(-15),
                            Gender = "Nam",
                            Address = "Chưa cập nhật"
                        });
                    }
                    else if (model.RoleID == 2) // Teacher
                    {
                        _context.Teachers.Add(new Teacher
                        {
                            UserID = model.UserID,
                            TeacherCode = "TC" + model.UserID.ToString("D4"),
                            Department = "Chưa cập nhật"
                        });
                    }

                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Thêm người dùng thành công!";
                    return RedirectToAction("Users");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                }
            }
            return View(model);
        }

        // Sửa người dùng (GET) - ĐÃ CẬP NHẬT
        [HttpGet]
        public IActionResult EditUser(int id)
        {
            var user = _context.Users
                .Include(u => u.Student)
                .Include(u => u.Teacher)
                .FirstOrDefault(u => u.UserID == id);

            if (user == null) return NotFound();
            return View(user);
        }

        // Sửa người dùng (POST) - ĐÃ CẬP NHẬT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(User model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy user hiện tại
                    var existingUser = _context.Users
                        .Include(u => u.Student)
                        .Include(u => u.Teacher)
                        .First(u => u.UserID == model.UserID);

                    // Cập nhật thông tin cơ bản
                    existingUser.FullName = model.FullName;
                    existingUser.Email = model.Email;
                    existingUser.Phone = model.Phone;
                    existingUser.RoleID = model.RoleID;
                    existingUser.IsActive = model.IsActive;

                    // Xử lý mật khẩu (chỉ cập nhật nếu có thay đổi)
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        existingUser.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
                    }

                    // Xử lý Student/Teacher khi thay đổi Role
                    if (model.RoleID == 3 && existingUser.Student == null)
                    {
                        _context.Students.Add(new Student
                        {
                            UserID = model.UserID,
                            StudentCode = "ST" + model.UserID.ToString("D4"),
                            DateOfBirth = DateTime.Today.AddYears(-15),
                            Gender = "Nam"
                        });

                        // Xóa Teacher nếu có
                        if (existingUser.Teacher != null)
                        {
                            _context.Teachers.Remove(existingUser.Teacher);
                        }
                    }
                    else if (model.RoleID == 2 && existingUser.Teacher == null)
                    {
                        _context.Teachers.Add(new Teacher
                        {
                            UserID = model.UserID,
                            TeacherCode = "TC" + model.UserID.ToString("D4")
                        });

                        // Xóa Student nếu có
                        if (existingUser.Student != null)
                        {
                            _context.Students.Remove(existingUser.Student);
                        }
                    }

                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Cập nhật thành công!";
                    return RedirectToAction("Users");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                }
            }
            return View(model);
        }

        // Xóa người dùng - ĐÃ CẬP NHẬT
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var user = _context.Users
                    .Include(u => u.Student)
                    .Include(u => u.Teacher)
                    .FirstOrDefault(u => u.UserID == id);

                if (user != null)
                {
                    // Xóa Student/Teacher trước
                    if (user.Student != null) _context.Students.Remove(user.Student);
                    if (user.Teacher != null) _context.Teachers.Remove(user.Teacher);

                    _context.Users.Remove(user);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Xóa người dùng thành công!";
                }
                return RedirectToAction("Users");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xóa: {ex.Message}";
                return RedirectToAction("Users");
            }
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

        // Quản lý lớp học
        public IActionResult Classes()
        {
            var classes = _context.Classes
                .Include(c => c.Teacher)
                .Select(c => new
                {
                    c.ClassID,
                    c.ClassName,
                    c.AcademicYear,
                    TeacherName = c.Teacher != null ? c.Teacher.User.FullName : "(Chưa có GVCN)",
                    StudentCount = c.Students.Count()
                })
                .ToList();

            return View(classes);
        }

        // Xem học sinh trong một lớp
        public IActionResult ClassDetails(int id)
        {
            var classInfo = _context.Classes
                .Include(c => c.Students)
                    .ThenInclude(s => s.User)
                .FirstOrDefault(c => c.ClassID == id);

            if (classInfo == null) return NotFound();

            var students = classInfo.Students
                .Select(s => new
                {
                    s.StudentID,
                    s.StudentCode,
                    FullName = s.User.FullName,
                    s.DateOfBirth,
                    s.Gender,
                    s.Address
                })
                .ToList();

            ViewBag.ClassName = classInfo.ClassName;
            ViewBag.AcademicYear = classInfo.AcademicYear;

            return View(students);
        }

        // Hiển thị danh sách môn học
        public IActionResult Subjects()
        {
            var subjects = _context.Subjects
                .Select(s => new Subject
                {
                    SubjectID = s.SubjectID,
                    SubjectName = s.SubjectName,
                    Credits = s.Credits,
                    Description = s.Description,
                    TeacherID = s.TeacherID,
                    TeacherName = s.TeacherID != null
                       ? _context.Teachers
                        .Where(t => t.TeacherID == s.TeacherID)
                        .Select(t => t.User.FullName)
                        .FirstOrDefault()
                        : "(Chưa phân công)",
                    GradeOrClass = s.GradeOrClass,
                    IsActive = s.IsActive
                })
                .ToList();

            return View(subjects);
        }


        // Tạo môn học mới (GET)
        [HttpGet]
        public IActionResult CreateSubject()
        {
            ViewBag.Teachers = _context.Teachers
        .Select(t => new { t.TeacherID, Name = t.User.FullName })
        .ToList();
            return View();
        }

        // Tạo môn học mới (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSubject(Subject model)
        {
            if (ModelState.IsValid)
            {
                _context.Subjects.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Subjects");
            }
            return View(model);
        }

        // Sửa môn học (GET)
        [HttpGet]
        public IActionResult EditSubject(int id)
        {
            var subject = _context.Subjects.FirstOrDefault(s => s.SubjectID == id);
            if (subject == null) return NotFound();
            return View(subject);
        }

        // Sửa môn học (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSubject(Subject model)
        {
            if (ModelState.IsValid)
            {
                _context.Subjects.Update(model);
                _context.SaveChanges();
                return RedirectToAction("Subjects");
            }
            return View(model);
        }

        // Xóa môn học
        public IActionResult DeleteSubject(int id)
        {
            var subject = _context.Subjects.FirstOrDefault(s => s.SubjectID == id);
            if (subject != null)
            {
                _context.Subjects.Remove(subject);
                _context.SaveChanges();
            }
            return RedirectToAction("Subjects");
        }

        // GET: Danh sách điểm số + lọc theo Semester
        public IActionResult Grades(int? semester, int? subjectId, int? classId, int page = 1, int pageSize = 10)
        {
            var gradesQuery = _context.Grades
                .Include(g => g.Student)
                    .ThenInclude(s => s.User)
                .Include(g => g.Student)
                    .ThenInclude(s => s.Class)
                .Include(g => g.Subject)
                .AsQueryable();

            // Lọc theo kỳ học
            if (semester.HasValue)
            {
                gradesQuery = gradesQuery.Where(g => g.Semester == semester.Value);
            }

            // Lọc theo môn học
            if (subjectId.HasValue)
            {
                gradesQuery = gradesQuery.Where(g => g.SubjectID == subjectId.Value);
            }

            // Lọc theo lớp học
            if (classId.HasValue)
            {
                gradesQuery = gradesQuery.Where(g => g.Student.ClassID == classId.Value);
            }

            // Thống kê
            ViewBag.TotalGrades = gradesQuery.Count();
            ViewBag.AverageScore = gradesQuery.Average(g => (double?)g.Score) ?? 0;
            ViewBag.StudentCount = gradesQuery.Select(g => g.StudentID).Distinct().Count();

            // Phân trang
            var totalRecords = gradesQuery.Count();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var grades = gradesQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Lấy danh sách cho dropdown
            ViewBag.Semesters = _context.Grades
                .Select(g => g.Semester)
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            ViewBag.Subjects = _context.Subjects.ToList();
            ViewBag.Classes = _context.Classes.ToList();

            // Giữ trạng thái lọc
            ViewBag.SelectedSemester = semester;
            ViewBag.SelectedSubjectId = subjectId;
            ViewBag.SelectedClassId = classId;
            ViewBag.PageNumber = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;

            return View(grades);
        }
    }
}
