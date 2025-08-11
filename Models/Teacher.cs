using System.Security.Claims;

namespace WebUseASP_test_.Models
{
    public class Teacher
    {
        public int TeacherID { get; set; }
        public int UserID { get; set; }
        public string TeacherCode { get; set; }
        public string Department { get; set; }
        public string Specialization { get; set; }

        public User User { get; set; }
        public ICollection<Class> Classes { get; set; }
    }
}
