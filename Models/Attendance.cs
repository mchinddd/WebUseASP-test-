namespace WebUseASP_test_.Models
{
    public class Attendance
    {
        public int AttendanceID { get; set; }
        public int StudentID { get; set; }
        public int ClassID { get; set; }
        public DateTime AttendanceDate { get; set; }
        public bool MorningSession { get; set; }
        public bool AfternoonSession { get; set; }
        public string Notes { get; set; }

        public Student Student { get; set; }
        public Class Class { get; set; }
    }
}
