namespace WebUseASP_test_.Models
{
    public class Class
    {
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public int? TeacherID { get; set; }
        public string AcademicYear { get; set; }

        public Teacher Teacher { get; set; }
        public ICollection<Student> Students { get; set; }
        public ICollection<Attendance> Attendances { get; set; }
    }
}
