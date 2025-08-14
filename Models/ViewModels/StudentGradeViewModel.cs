namespace WebUseASP_test_.ViewModels
{
    public class StudentGradeViewModel
    {
        public string SubjectName { get; set; }
        public double? Grade15 { get; set; }
        public double? Grade1Tiet { get; set; }
        public double? FinalExam { get; set; }
        public double Average => Math.Round(
            (new[] { Grade15, Grade1Tiet, FinalExam }
                .Where(g => g.HasValue)
                .Select(g => g.Value)
                .DefaultIfEmpty(0)
                .Average()), 2);
    }

    public class GradesPageViewModel
    {
        public int SelectedSemester { get; set; }
        public List<StudentGradeViewModel> Grades { get; set; }
    }
}
