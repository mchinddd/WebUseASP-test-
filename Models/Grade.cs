namespace WebUseASP_test_.Models
{
    public class Grade
    {
        public int GradeID { get; set; }
        public int StudentID { get; set; }
        public int SubjectID { get; set; }
        public double Score { get; set; }
        public int? Semester { get; set; }
        public DateTime CreatedDate { get; set; }

        public Student Student { get; set; }
        public Subject Subject { get; set; }
    }
}
