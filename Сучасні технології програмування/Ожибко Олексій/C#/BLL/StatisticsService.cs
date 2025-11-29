namespace BLL
{
    using DB_Connection;

    public class StatisticsService
    {
        private readonly JournalDbContext _context;

        public StatisticsService(JournalDbContext context)
        {
            this._context = context;
        }

        public List<UserCreationStatistic> GetUserCreationData()
        {
            return this._context.Users
                .Where(u => u.RoleId == 1 || u.RoleId == 2)
                .GroupBy(u => new { u.DateCreated.Date, u.RoleId })
                .Select(g => new UserCreationStatistic
                {
                    Date = g.Key.Date,
                    RoleId = g.Key.RoleId,
                    Count = g.Count(),
                })
                .OrderBy(x => x.Date)
                .ToList();
        }

        public GradesStatistic GetGradesData()
        {
            var totalGradesCount = this._context.StudentPerformances
                .Count(sp => sp.Grade.HasValue);

            var excellentGradesCount = totalGradesCount > 0
                ? this._context.StudentPerformances
                    .Count(sp => sp.Grade >= 10 && sp.Grade <= 12)
                : 0;

            return new GradesStatistic
            {
                ExcellentGradesCount = excellentGradesCount,
                TotalGradesCount = totalGradesCount,
            };
        }
    }

    public class UserCreationStatistic
    {
        public DateTime Date { get; set; }

        public int RoleId { get; set; }

        public int Count { get; set; }
    }

    public class GradesStatistic
    {
        public int ExcellentGradesCount { get; set; }

        public int TotalGradesCount { get; set; }
    }
}