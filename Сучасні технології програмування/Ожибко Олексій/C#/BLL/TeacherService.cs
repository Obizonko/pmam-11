namespace BLL
{
    using DB_Connection;
    using Microsoft.EntityFrameworkCore;

    public class TeacherService
    {
        private readonly JournalDbContext _context;

        public TeacherService(JournalDbContext context)
        {
            this._context = context;
        }

        public void UpdateSchedule(Schedule schedule)
        {
            var existingSchedule = this._context.Schedules.Find(schedule.ScheduleId);
            if (existingSchedule != null)
            {
                this._context.Entry(existingSchedule).CurrentValues.SetValues(schedule);
                this._context.SaveChanges();
            }
        }

        public void DeleteTeacher(int teacherId)
        {
            var teacherSubjects = this._context.TeacherSubjects.Where(ts => ts.TeacherId == teacherId).ToList();
            this._context.TeacherSubjects.RemoveRange(teacherSubjects);

            var teacher = this._context.Users.Find(teacherId);
            if (teacher != null)
            {
                this._context.Users.Remove(teacher);
            }

            this._context.SaveChanges();
        }

        public void UpdateTeacher(User teacher)
        {
            var existingTeacher = this._context.Users.Find(teacher.UserId);
            if (existingTeacher != null)
            {
                this._context.Entry(existingTeacher).CurrentValues.SetValues(teacher);
                this._context.SaveChanges();
            }
        }

        public void AddNewTeacher(User teacher, List<string> subjectNames)
        {
            this._context.Users.Add(teacher);
            this._context.SaveChanges();

            int teacherId = teacher.UserId;

            foreach (var subjectName in subjectNames)
            {
                var subject = this._context.Subjects.FirstOrDefault(s => s.SubjectName == subjectName);
                if (subject != null)
                {
                    var teacherSubject = new TeacherSubject
                    {
                        TeacherId = teacherId,
                        SubjectId = subject.SubjectId,
                    };
                    this._context.TeacherSubjects.Add(teacherSubject);
                }
            }

            this._context.SaveChanges();
        }

        public List<string> GetAllSubjects()
        {
            return this._context.Subjects.Select(s => s.SubjectName).ToList();
        }

        public void AddNewSchedule(Schedule schedule)
        {
            this._context.Schedules.Add(schedule);
            this._context.SaveChanges();
        }

        public List<User> GetAllTeachers()
        {
            return this._context.Users
                .Where(u => u.RoleId == 2)
                .ToList();
        }

        public List<Schedule> GetScheduleForTeacher(int teacherId, string dayOfWeek)
        {
            return this._context.Schedules
                .AsNoTracking()
                .Where(s => s.TeacherId == teacherId && s.DayOfWeek == dayOfWeek)
                .Include(s => s.Subject)
                .Include(s => s.ClassGroup)
                .OrderBy(s => s.StartTime)
                .ToList();
        }

        public int GetSubjectIdByName(string subjectName)
        {
            var subject = this._context.Subjects.FirstOrDefault(s => s.SubjectName == subjectName);
            return subject?.SubjectId ?? -1;
        }

        public int GetClassGroupIdByName(string classGroupName)
        {
            var classGroup = this._context.ClassGroups.FirstOrDefault(cg => cg.ClassGroupName == classGroupName);
            return classGroup?.ClassGroupId ?? -1;
        }
    }
}