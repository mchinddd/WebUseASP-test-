using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using WebUseASP_test_.Data; // namespace DbContext
using WebUseASP_test_.Models;
using WebUseASP_test_.Models.ViewModels;
using WebUseASP_test_.ViewModels;


namespace WebUseASP_test_.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            ViewData["PageIcon"] = "fa-home";
            ViewData["PageTitle"] = "Trang chủ sinh viên";
            return View();
        }
        public async Task<IActionResult> Profile()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Index", "Login"); 

            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest("UserID không hợp lệ.");

            var student = await _context.Students
                .Include(s => s.User)
                .Include(s => s.Class)
                .Include(s => s.Grades)
                .Include(s => s.Attendances)
                .FirstOrDefaultAsync(s => s.UserID == userId);

            if (student == null)
                return NotFound("Không tìm thấy thông tin học sinh.");

            // Đảm bảo List không null
            var grades = student.Grades ?? new List<Grade>();
            var attendances = student.Attendances ?? new List<Attendance>();

            double avgScore = grades.Any() ? grades.Average(g => g.Score) : 0;
            int subjectsCount = grades.Select(g => g.SubjectID).Distinct().Count();

            int totalSessions = attendances.Count * 2;
            int presentSessions = attendances.Sum(a => (a.MorningSession ? 1 : 0) + (a.AfternoonSession ? 1 : 0));

            double attendanceRate = totalSessions > 0
                ? (presentSessions / (double)totalSessions) * 100
                : 0;

            var vm = new StudentProfileViewModel
            {
                FullName = student.User?.FullName ?? "N/A",
                StudentCode = student.StudentCode,
                ClassName = student.Class?.ClassName ?? "Chưa có lớp",
                DateOfBirth = student.DateOfBirth,
                Gender = student.Gender,
                Address = student.Address,
                Phone = student.User?.Phone ?? "N/A",
                AverageScore = Math.Round(avgScore, 2),
                SubjectsCount = subjectsCount,
                AttendanceRate = Math.Round(attendanceRate, 2)
            };

            return View(vm);
        }

        public async Task<IActionResult> Grades(int semester = 1)
        {
            // Lấy UserID từ claims
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Index", "Login");

            int userId = int.Parse(userIdClaim);

            // Lấy StudentID của user
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserID == userId);

            if (student == null)
                return NotFound();

            // Lấy danh sách điểm của học sinh theo học kỳ
            var grades = await _context.Grades
                .Include(g => g.Subject)
                .Where(g => g.StudentID == student.StudentID && g.Semester == semester)
                .ToListAsync();

            // Giả sử hiện tại bảng Grades chỉ có 1 điểm duy nhất cho mỗi môn
            // Nếu bạn tách loại điểm (15', 1 tiết, thi) thì cần cột riêng để phân biệt
            var vm = new GradesPageViewModel
            {
                SelectedSemester = semester,
                Grades = grades.Select(g => new StudentGradeViewModel
                {
                    SubjectName = g.Subject.SubjectName,
                    Grade15 = g.Score, // giả sử Score = điểm tổng hợp, sẽ cần tách nếu muốn chi tiết hơn
                    Grade1Tiet = null, // bạn có thể map từ DB nếu có
                    FinalExam = null
                }).ToList()
            };

            return View(vm);
        }

        public async Task<IActionResult> Schedule(int? weekOffset = 0)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Index", "Login");

            int userId = int.Parse(userIdClaim);

            // Lấy StudentID
            var student = await _context.Students
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s => s.UserID == userId);

            if (student == null)
                return NotFound();

            // Tính toán ngày bắt đầu và kết thúc của tuần dựa trên offset
            DateTime today = DateTime.Today;
            int currentDayOfWeek = (int)today.DayOfWeek;
            DateTime startOfWeek = today.AddDays(-(currentDayOfWeek + 6) % 7); // Thứ 2 đầu tuần
            startOfWeek = startOfWeek.AddDays(7 * weekOffset.Value);
            DateTime endOfWeek = startOfWeek.AddDays(6); // Chủ nhật cuối tuần

            // Lấy thời khóa biểu của lớp đó
            var schedules = await _context.Schedules
                .Include(s => s.Subject)
                .Include(s => s.Teacher)
                    .ThenInclude(t => t.User)
                .Where(s => s.ClassID == student.ClassID)
                .OrderBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            // Truyền thêm thông tin về tuần để hiển thị
            ViewBag.StartOfWeek = startOfWeek;
            ViewBag.EndOfWeek = endOfWeek;
            ViewBag.WeekOffset = weekOffset;

            return View(schedules);
        }

        public async Task<IActionResult> Attendance()
        {
            // Lấy UserID hiện tại từ Claims
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Lấy thông tin học sinh
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserID == userId);

            if (student == null)
                return NotFound("Không tìm thấy học sinh.");

            // Lấy lịch sử điểm danh của học sinh
            var attendances = await _context.Attendances
                .Include(a => a.Class)
                .Where(a => a.StudentID == student.StudentID)
                .OrderByDescending(a => a.AttendanceDate)
                .ToListAsync();

            return View(attendances);
        }
    }
}
