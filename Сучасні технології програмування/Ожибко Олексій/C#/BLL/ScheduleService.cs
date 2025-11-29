namespace BLL
{
    using DB_Connection;
    using Microsoft.EntityFrameworkCore;

    public class ScheduleService
    {
        private readonly JournalDbContext _context;

        public ScheduleService(JournalDbContext context)
        {
            this._context = context;
        }

        public List<ScheduleDetail> GetScheduleForClassGroupAndDay(int classGroupId, DayOfWeek dayOfWeek, int studentId, DateTime selectedDate)
        {
            string dayName = this.GetDayOfWeekInUkrainian(dayOfWeek);
            var schedules = this._context.Schedules
                .Include(s => s.Subject)
                .Where(s => s.ClassGroupId == classGroupId && s.DayOfWeek == dayName)
                .ToList();

            var scheduleDetails = new List<ScheduleDetail>();
            foreach (var s in schedules)
            {
                var studentPerformance = this._context.StudentPerformances
                    .FirstOrDefault(sp => sp.StudentId == studentId &&
                                          sp.SubjectId == s.SubjectId &&
                                          sp.ScheduleId == s.ScheduleId &&
                                          sp.Date == selectedDate);

                scheduleDetails.Add(new ScheduleDetail
                {
                    ScheduleId = s.ScheduleId,
                    SubjectName = s.Subject.SubjectName,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Status = studentPerformance?.Status ?? "Присутній",
                    Grade = studentPerformance?.Grade,
                });
            }

            return scheduleDetails;
        }

        private string GetDayOfWeekInUkrainian(DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Monday => "Понедiлок",
                DayOfWeek.Tuesday => "Вiвторок",
                DayOfWeek.Wednesday => "Середа",
                DayOfWeek.Thursday => "Четвер",
                DayOfWeek.Friday => "П'ятниця",
                DayOfWeek.Saturday => "Субота",
                DayOfWeek.Sunday => "Неділя",
                _ => throw new ArgumentOutOfRangeException(nameof(day), $"Неіснуючий день тижня: {day}"),
            };
        }
    }

    public class ScheduleDetail
    {
        public int ScheduleId { get; set; }

        public string? SubjectName { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string? Status { get; set; }

        public int? Grade { get; set; }
    }
}