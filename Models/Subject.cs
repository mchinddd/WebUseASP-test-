using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace WebUseASP_test_.Models
{
    public class Subject
    {
        public int SubjectID { get; set; }
        public string SubjectName { get; set; }
        public string Description { get; set; }
        public int Credits { get; set; }
        public int? TeacherID { get; set; }

        [NotMapped] // Không lưu trong DB, chỉ dùng để hiển thị
        public string TeacherName { get; set; } // dùng để hiển thị
        public string GradeOrClass { get; set; }
        public bool IsActive { get; set; }



        public ICollection<Grade> Grades { get; set; }
    }
}
