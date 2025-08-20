using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebUseASP_test_.Models.ViewModels
{
    public class EditClassViewModel
    {
        
            public int ClassID { get; set; }
            public string ClassName { get; set; }
            public string AcademicYear { get; set; }
            public int? TeacherID { get; set; }

            public List<int> SelectedStudentIDs { get; set; } = new List<int>();

            public List<SelectListItem> Teachers { get; set; } = new();
            public List<SelectListItem> Students { get; set; } = new();
        }
}
