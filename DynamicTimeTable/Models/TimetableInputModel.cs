using System.ComponentModel.DataAnnotations;

namespace DynamicTimeTable.Models
{
    public class TimetableInputModel
    {
        [Range(1, 7)]
        public int WorkingDays { get; set; }

        [Range(1, 8)]
        public int SubjectsPerDay { get; set; }

        [Range(1, 20)]
        public int TotalSubjects { get; set; }

        public int TotalHours => WorkingDays * SubjectsPerDay;
    }

    public class SubjectHoursModel
    {
        public string SubjectName { get; set; }

        [Range(1, 100)]
        public int WeeklyHours { get; set; }
    }
}
