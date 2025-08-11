using System.Diagnostics;

namespace WebUseASP_test_.Models
{
    public class Subject
    {
        public int SubjectID { get; set; }
        public string SubjectName { get; set; }
        public int Credits { get; set; }
        public string Description { get; set; }

        public ICollection<Grade> Grades { get; set; }
    }
}
