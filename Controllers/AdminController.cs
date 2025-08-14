using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebUseASP_test_.Data;
using WebUseASP_test_.Models;
using WebUseASP_test_.ViewModels;

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
            var users = _context.Users
        .Include(u => u.Student)
        .Include(u => u.Teacher)
        .Include(u => u.Role)
        .ToList();

            return View(users);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (ModelState.IsValid)
            {
                user.CreatedDate = DateTime.Now;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm người dùng thành công!";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Có lỗi xảy ra, vui lòng kiểm tra lại thông tin.";
            return View(user);
        }




        // GET: Hiển thị form sửa user
        [HttpGet]
        public IActionResult EditUser(int id)
        {
            var user = _context.Users
                .Include(u => u.Student)
                .Include(u => u.Teacher)
                .FirstOrDefault(u => u.UserID == id);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Người dùng không tồn tại!";
                return RedirectToAction("Users");
            }

            return View(user);
        }


        [HttpPost]
        public IActionResult EditUser(User model)
        {
            // Lấy entity gốc từ DB
            var user = _context.Users
                .Include(u => u.Student)
                .Include(u => u.Teacher)
                .FirstOrDefault(u => u.UserID == model.UserID);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Người dùng không tồn tại!";
                return RedirectToAction("Users");
            }

            // Cập nhật thông tin User
            user.Username = model.Username;
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Phone = model.Phone;
            user.RoleID = model.RoleID;
            user.IsActive = model.IsActive;

            // Cập nhật thông tin học sinh nếu có
            if (user.Student != null && model.Student != null)
            {
                user.Student.StudentCode = model.Student.StudentCode;
                user.Student.DateOfBirth = model.Student.DateOfBirth;
                user.Student.Gender = model.Student.Gender;
                user.Student.Address = model.Student.Address;
                user.Student.ClassID = model.Student.ClassID;
            }

            // Cập nhật thông tin giáo viên nếu có
            if (user.Teacher != null && model.Teacher != null)
            {
                user.Teacher.TeacherCode = model.Teacher.TeacherCode;
                user.Teacher.Department = model.Teacher.Department;
                user.Teacher.Specialization = model.Teacher.Specialization;
            }

            _context.SaveChanges(); // Quan trọng: lưu vào DB
            TempData["SuccessMessage"] = "Cập nhật thành công!";
            return RedirectToAction("Users");
        }



        // Xóa người dùng
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Student)
                    .ThenInclude(s => s.Grades)
                .Include(u => u.Student)
                    .ThenInclude(s => s.Attendances)
                .Include(u => u.Teacher)
                    .ThenInclude(t => t.Classes)
                .FirstOrDefaultAsync(u => u.UserID == id);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Người dùng không tồn tại!";
                return RedirectToAction("Users");
            }

            // Xóa dữ liệu liên quan
            if (user.Student != null)
            {
                if (user.Student.Grades != null)
                    _context.Grades.RemoveRange(user.Student.Grades);

                if (user.Student.Attendances != null)
                    _context.Attendances.RemoveRange(user.Student.Attendances);

                _context.Students.Remove(user.Student);
            }

            if (user.Teacher != null)
            {
                if (user.Teacher.Classes != null)
                    _context.Classes.RemoveRange(user.Teacher.Classes);

                _context.Teachers.Remove(user.Teacher);
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa người dùng thành công!";
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

        [HttpPost]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            // Lấy giáo viên và dữ liệu liên quan
            var teacher = await _context.Teachers
                .Include(t => t.Classes)
                .Include(t => t.User)
                 // nếu có bảng Schedules
                .FirstOrDefaultAsync(t => t.TeacherID == id);

            if (teacher == null)
            {
                TempData["ErrorMessage"] = "Giáo viên không tồn tại!";
                return RedirectToAction("Teachers");
            }

            // Kiểm tra dữ liệu liên quan
            bool hasClasses = teacher.Classes != null && teacher.Classes.Any();
            //bool hasSchedules = teacher.Schedules != null && teacher.Schedules.Any();

            if (hasClasses)
            {
                TempData["ErrorMessage"] = "Không thể xóa giáo viên này vì đang dạy lớp hoặc có lịch!";
                return RedirectToAction("Teachers");
            }

            // Nếu không có dữ liệu liên quan, xóa User và Teacher
            if (teacher.User != null)
                _context.Users.Remove(teacher.User);

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa giáo viên thành công!";
            return RedirectToAction("Teachers");
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
