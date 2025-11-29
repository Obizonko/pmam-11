namespace BLL;

using DB_Connection;

public class UserService
{
    private readonly JournalDbContext _context;

    public UserService(JournalDbContext context)
    {
        this._context = context;
    }

    public void DeleteSchedule(Schedule schedule)
    {
        var existingSchedule = this._context.Schedules.Find(schedule.ScheduleId);
        if (existingSchedule != null)
        {
            this._context.Schedules.Remove(existingSchedule);
            this._context.SaveChanges();
        }
    }

    public int GetTeachersCount()
    {
        return this._context.Users.Count(u => u.RoleId == 2);
    }

    public int GetStudentsCount()
    {
        return this._context.Users.Count(u => u.RoleId == 1);
    }
}
