using System.Diagnostics;
using System.Security.Claims;

namespace WebUseASP_test_.Models
{
    public class Student
    {
        public int StudentID { get; set; }
        public int UserID { get; set; }
        public string StudentCode { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public int ClassID { get; set; }

        public User User { get; set; }
        public Class Class { get; set; }
        public ICollection<Grade> Grades { get; set; }
        public ICollection<Attendance> Attendances { get; set; }
    }
}
