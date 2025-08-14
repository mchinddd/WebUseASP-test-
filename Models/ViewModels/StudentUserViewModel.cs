using WebUseASP_test_.Models;

namespace WebUseASP_test_.ViewModels
{
    public class StudentUserViewModel
    {
        public User User { get; set; } = new User();
        public Student Student { get; set; } = new Student();
    }
}