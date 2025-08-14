namespace WebUseASP_test_.Models.ViewModels
{
    public class StudentProfileViewModel
    {
        public string FullName { get; set; }
        public string StudentCode { get; set; }
        public string ClassName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        // Thống kê
        public double AverageScore { get; set; }
        public int SubjectsCount { get; set; }
        public double AttendanceRate { get; set; }
    }
}
