using System;

namespace WebUseASP_test_.Models
{
    public class Schedule
    {
        public int ScheduleID { get; set; }

        public int ClassID { get; set; } // Lớp học nào
        public int SubjectID { get; set; } // Môn học
        public int? TeacherID { get; set; } // Giáo viên

        public int DayOfWeek { get; set; } // 2=Thứ 2, 3=Thứ 3,...
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        // Navigation properties
        public Class Class { get; set; }
        public Subject Subject { get; set; }
        public Teacher Teacher { get; set; }
    }
}
