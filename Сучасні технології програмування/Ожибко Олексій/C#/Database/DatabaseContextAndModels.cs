namespace DB_Connection
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    public class JournalDbContext : DbContext
    {
        public JournalDbContext()
            : base(GetOptions())
        {
        }

        public JournalDbContext(DbContextOptions<JournalDbContext> options)
            : base(options)
        {
        }

        private static DbContextOptions<JournalDbContext> GetOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<JournalDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Udik;Username=postgres;Password=11111111;");
            return optionsBuilder.Options;
        }

        public DbSet<Role> Roles { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Subject> Subjects { get; set; }

        public DbSet<ClassGroup> ClassGroups { get; set; }

        public DbSet<TeacherSubject> TeacherSubjects { get; set; }

        public DbSet<StudentPerformance> StudentPerformances { get; set; }

        public virtual DbSet<Schedule> Schedules { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Udik;Username=postgres;Password=11111111;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurations
            modelBuilder.Entity<TeacherSubject>()
                .HasKey(ts => new { ts.TeacherId, ts.SubjectId });

            modelBuilder.Entity<TeacherSubject>()
                .HasOne(ts => ts.Teacher)
                .WithMany(u => u.TeacherSubjects)
                .HasForeignKey(ts => ts.TeacherId);

            modelBuilder.Entity<TeacherSubject>()
                .HasOne(ts => ts.Subject)
                .WithMany(s => s.TeacherSubjects)
                .HasForeignKey(ts => ts.SubjectId);

            modelBuilder.Entity<StudentPerformance>()
                .HasOne(sp => sp.Student)
                .WithMany(u => u.StudentPerformances)
                .HasForeignKey(sp => sp.StudentId);

            modelBuilder.Entity<StudentPerformance>()
                .HasOne(sp => sp.Subject)
                .WithMany(s => s.StudentPerformances)
                .HasForeignKey(sp => sp.SubjectId);

            modelBuilder.Entity<StudentPerformance>()
                .HasOne(sp => sp.Schedule)
                .WithMany(sch => sch.StudentPerformances)
                .HasForeignKey(sp => sp.ScheduleId);

            modelBuilder.Entity<Schedule>()
                .HasOne(sch => sch.Subject)
                .WithMany(s => s.Schedules)
                .HasForeignKey(sch => sch.SubjectId);

            modelBuilder.Entity<Schedule>()
                .HasOne(sch => sch.ClassGroup)
                .WithMany(cg => cg.Schedules)
                .HasForeignKey(sch => sch.ClassGroupId);
        }
    }

    // Entity definitions
    [Table("studentperformance")]
    public class StudentPerformance
    {
        [Column("attendance_id")]
        public int StudentPerformanceId { get; set; }

        [Column("student_id")]
        public int StudentId { get; set; }

        public User Student { get; set; }

        [Column("subject_id")]
        public int SubjectId { get; set; }

        public Subject Subject { get; set; }

        [Column("schedule_id")]
        public int ScheduleId { get; set; }

        public Schedule Schedule { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("grade")]
        public int? Grade { get; set; }
    }

    [Table("roles")]
    public class Role
{
    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("role_name")]
    public string RoleName { get; set; }

    public List<User> Users { get; set; }
}

    [Table("users")]
    public class User
{
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("username")]
    public string Username { get; set; }

    [Column("password")]
    public string Password { get; set; }

    [Column("email")]
    public string Email { get; set; }

    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("first_name")]
    public string FirstName { get; set; }

    [Column("last_name")]
    public string LastName { get; set; }

    public string FullName => $"{this.FirstName} {this.LastName}";

    [Column("class_group_id")]
    public int? ClassGroupId { get; set; }

    public virtual ClassGroup ClassGroup { get; set; }

    [Column("date_created")]
    public DateTime DateCreated { get; set; }

    public Role Role { get; set; }

    public List<TeacherSubject> TeacherSubjects { get; set; }

    public List<StudentPerformance> StudentPerformances { get; set; }
}

    [Table("subjects")]
    public class Subject
{
    [Column("subject_id")]
    public int SubjectId { get; set; }

    [Column("subject_name")]
    public string SubjectName { get; set; }

    public List<TeacherSubject> TeacherSubjects { get; set; }

    public List<StudentPerformance> StudentPerformances { get; set; }

    public List<Schedule> Schedules { get; set; }
}

    [Table("classgroups")]
    public class ClassGroup
{
    [Column("class_group_id")]
    public int ClassGroupId { get; set; }

    [Column("class_group_name")]
    public string ClassGroupName { get; set; }

    public List<Schedule> Schedules { get; set; }
}

    [Table("teachersubjects")]
    public class TeacherSubject
{
    [Column("teacher_id")]
    public int TeacherId { get; set; }

    [Column("subject_id")]
    public int SubjectId { get; set; }

    public User Teacher { get; set; }

    public Subject Subject { get; set; }
}

    [Table("schedule")]
    public class Schedule
{
    [Column("schedule_id")]
    public int ScheduleId { get; set; }

    [Column("subject_id")]
    public int SubjectId { get; set; }

    [Column("class_group_id")]
    public int ClassGroupId { get; set; }

    [Column("teacher_id")]
    public int TeacherId { get; set; }

    [Column("day_of_week")]
    public string DayOfWeek { get; set; }

    [Column("start_time")]
    public TimeSpan StartTime { get; set; }

    [Column("end_time")]
    public TimeSpan EndTime { get; set; }

    public Subject Subject { get; set; }

    public ClassGroup ClassGroup { get; set; }

    public User Teacher { get; set; }

    public List<StudentPerformance> StudentPerformances { get; set; }

    [NotMapped]
    public bool IsNew { get; set; }
}
}
