namespace BLL
{
    using DB_Connection;
    using Microsoft.EntityFrameworkCore;

    public class StudentService
    {
        private readonly JournalDbContext _context;

        public StudentService(JournalDbContext context)
        {
            this._context = context;
        }

        public void AddNewStudent(User student)
        {
            this._context.Users.Add(student);
            this._context.SaveChanges();
        }

        public List<ClassGroup> GetAllClassGroups()
        {
            return this._context.ClassGroups.ToList();
        }

        public int GetClassGroupIdByName(string classGroupName)
        {
            var classGroup = this._context.ClassGroups.FirstOrDefault(cg => cg.ClassGroupName == classGroupName);
            return classGroup?.ClassGroupId ?? -1;
        }

        public List<User> GetAllStudents()
        {
            return this._context.Users
                .Where(u => u.RoleId == 1)
                .Include(u => u.ClassGroup)
                .ToList();
        }

        public List<User> GetStudentsByClassGroup(int classGroupId)
        {
            return this._context.Users
                .Where(u => u.RoleId == 1 && u.ClassGroupId == classGroupId)
                .Include(u => u.ClassGroup)
                .ToList();
        }

        public void UpdateStudent(User student)
        {
            var existingStudent = this._context.Users.Find(student.UserId);
            if (existingStudent != null)
            {
                this._context.Entry(existingStudent).CurrentValues.SetValues(student);
                this._context.SaveChanges();
            }
        }

        public int GetTotalGradesCountByClassGroup(int classGroupId)
        {
            return this._context.StudentPerformances
                .Count(sp => sp.Student.ClassGroupId == classGroupId && sp.Grade.HasValue && sp.Grade.Value > 0);
        }

        public int GetTotalGradesCountByStudent(int studentId)
        {
            return this._context.StudentPerformances
                .Count(sp => sp.StudentId == studentId && sp.Grade.HasValue && sp.Grade.Value > 0);
        }

        public int GetAbsencesCountByStudent(int studentId)
        {
            return this._context.StudentPerformances.Count(sp => sp.StudentId == studentId && sp.Status == " н ");
        }

        public int GetGradeCountByStudent(int studentId, int minGrade, int maxGrade)
        {
            return this._context.StudentPerformances.Count(sp => sp.StudentId == studentId && sp.Grade >= minGrade && sp.Grade <= maxGrade);
        }

        public int GetStudentCountByClassGroup(int classGroupId)
        {
            return this._context.Users.Count(u => u.ClassGroupId == classGroupId && u.RoleId == 1);
        }

        public int GetTotalAbsencesByClassGroup(int classGroupId)
        {
            return this._context.StudentPerformances
                .Count(sp => sp.Student.ClassGroupId == classGroupId && sp.Status == " н ");
        }

        public int GetGradeCountByClassGroup(int classGroupId, int minGrade, int maxGrade)
        {
            return this._context.StudentPerformances
                .Count(sp => sp.Student.ClassGroupId == classGroupId && sp.Grade >= minGrade && sp.Grade <= maxGrade);
        }

        public void DeleteStudent(int userId)
        {
            var student = this._context.Users.Find(userId);
            if (student != null)
            {
                this._context.Users.Remove(student);
                this._context.SaveChanges();
            }
        }

        public void SaveChanges()
        {
            this._context.SaveChanges();
        }
    }
}