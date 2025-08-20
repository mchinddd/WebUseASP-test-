using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebUseASP_test_.Data;
using WebUseASP_test_.Models;
using WebUseASP_test_.Models.ViewModels;

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

        // GET: CreateUser
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        // POST: CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUser(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra username tồn tại
            if (_context.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Username đã tồn tại");
                return View(model);
            }

            // Kiểm tra email tồn tại
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại");
                return View(model);
            }

            var user = new User
            {
                Username = model.Username,
                Password = model.Password,   // TODO: Hash password
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                RoleID = model.RoleId,     // User model dùng RoleID
                IsActive = model.IsActive,
                CreatedDate = DateTime.Now     // tránh lỗi datetime MinValue
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Thêm người dùng thành công!";
            return RedirectToAction("Users");
        }

        public IActionResult UserList()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        // GET: EditUser
        [HttpGet]
        public IActionResult EditUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // Sửa người dùng (POST) 
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


        //xoa nguoi dung post
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
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTeacherConfirmed([FromBody] DeleteRequest request)
        {
            int id = request.Id;

            var teacher = _context.Teachers.FirstOrDefault(t => t.TeacherID == id);
            if (teacher == null)
            {
                return Json(new { success = false, message = "Không tìm thấy giáo viên." });
            }

            bool isTeaching = _context.Subjects.Any(c => c.TeacherID == id)
                           || _context.Classes.Any(cl => cl.TeacherID == id);

            if (isTeaching)
            {
                return Json(new { success = false, message = "Không thể xóa! Giáo viên này đang dạy môn hoặc lớp." });
            }

            _context.Teachers.Remove(teacher);
            _context.SaveChanges();

            return Json(new { success = true, message = "Xóa giáo viên thành công!" });
        }

        public class DeleteRequest
        {
            public int Id { get; set; }
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

        // GET: EditClass
        [HttpGet]
        public IActionResult EditClass(int id)
        {
            var classInfo = _context.Classes
                .Include(c => c.Students)
                    .ThenInclude(s => s.User)
                .FirstOrDefault(c => c.ClassID == id);
            if (classInfo == null) return NotFound();

            var teachers = _context.Teachers
                .Include(t => t.User)
                .Select(t => new { t.TeacherID, FullName = t.User.FullName })
                .ToList();

            var studentsWithoutClass = _context.Students
                .Include(s => s.User)
                .Where(s => s.ClassID == null)
                .Select(s => new { s.StudentID, FullName = s.User.FullName })
                .ToList();

            var currentStudents = classInfo.Students
                .Select(s => new { s.StudentID, FullName = s.User.FullName })
                .ToList();

            // Gộp 2 nhóm, loại trùng
            var allAvailableStudents = currentStudents
                .Concat(studentsWithoutClass)
                .GroupBy(x => x.StudentID)
                .Select(g => g.First())
                .OrderBy(x => x.FullName)
                .ToList();

            var model = new EditClassViewModel
            {
                ClassID = classInfo.ClassID,
                ClassName = classInfo.ClassName,
                AcademicYear = classInfo.AcademicYear,
                TeacherID = classInfo.TeacherID,
                SelectedStudentIDs = classInfo.Students.Select(s => s.StudentID).ToList(),
                Teachers = teachers.Select(t => new SelectListItem { Value = t.TeacherID.ToString(), Text = t.FullName }).ToList(),
                Students = allAvailableStudents.Select(s => new SelectListItem { Value = s.StudentID.ToString(), Text = s.FullName }).ToList()
            };

            return View(model);
        }


        // POST: EditClass
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditClass(EditClassViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // IMPORTANT: nạp lại lists nếu return View(model)
                return ReloadEditClassListsAndReturnView(model);
            }

            var classInfo = _context.Classes.FirstOrDefault(c => c.ClassID == model.ClassID);
            if (classInfo == null) return NotFound();

            classInfo.ClassName = model.ClassName;
            classInfo.AcademicYear = model.AcademicYear;
            classInfo.TeacherID = model.TeacherID;

            var selected = model.SelectedStudentIDs ?? new List<int>();

            // Những HS đang thuộc lớp nhưng bị bỏ chọn -> set NULL
            var toRemove = _context.Students.Where(s => s.ClassID == classInfo.ClassID && !selected.Contains(s.StudentID)).ToList();
            foreach (var s in toRemove) s.ClassID = null;

            // Những HS được chọn -> gán vào lớp (kể cả trước đó thuộc lớp khác)
            var toAdd = _context.Students.Where(s => selected.Contains(s.StudentID)).ToList();
            foreach (var s in toAdd) s.ClassID = classInfo.ClassID;

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Cập nhật lớp học thành công!";
            return RedirectToAction("Classes");
        }

        // helper để nạp lại dropdown khi ModelState invalid
        private IActionResult ReloadEditClassListsAndReturnView(EditClassViewModel model)
        {
            var classInfo = _context.Classes
                .Include(c => c.Students)
                    .ThenInclude(s => s.User)
                .FirstOrDefault(c => c.ClassID == model.ClassID);
            if (classInfo == null) return NotFound();

            var teachers = _context.Teachers
                .Include(t => t.User)
                .Select(t => new { t.TeacherID, FullName = t.User.FullName })
                .ToList();

            var studentsWithoutClass = _context.Students
                .Include(s => s.User)
                .Where(s => s.ClassID == null)
                .Select(s => new { s.StudentID, FullName = s.User.FullName })
                .ToList();

            var currentStudents = classInfo.Students
                .Select(s => new { s.StudentID, FullName = s.User.FullName })
                .ToList();

            var all = currentStudents.Concat(studentsWithoutClass)
                .GroupBy(x => x.StudentID).Select(g => g.First())
                .OrderBy(x => x.FullName).ToList();

            model.Teachers = teachers.Select(t => new SelectListItem { Value = t.TeacherID.ToString(), Text = t.FullName }).ToList();
            model.Students = all.Select(s => new SelectListItem { Value = s.StudentID.ToString(), Text = s.FullName }).ToList();

            return View("EditClass", model);
        }

        //delte class
        public IActionResult DeleteClass(int id)
        {
            var classInfo = _context.Classes
                .Include(c => c.Students)
                .FirstOrDefault(c => c.ClassID == id);

            if (classInfo == null) return NotFound();

            if (classInfo.TeacherID != null || classInfo.Students.Any())
            {
                TempData["ErrorMessage"] = "Không thể xóa lớp vì còn giáo viên hoặc học sinh!";
                return RedirectToAction("Classes");
            }

            _context.Classes.Remove(classInfo);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Xóa lớp thành công!";
            return RedirectToAction("Classes");
        }

        //xoa hoc sinh khoi lop
        [HttpPost]
        public IActionResult RemoveStudentFromClass(int studentId, int classId)
        {
            var student = _context.Students.FirstOrDefault(s => s.StudentID == studentId);

            if (student != null)
            {
                // gỡ học sinh ra khỏi lớp
                student.ClassID = null;
                _context.SaveChanges();
            }

            // Sau khi xóa xong thì quay lại danh sách học sinh trong lớp đó
            return RedirectToAction("Classes", new { id = classId });
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

