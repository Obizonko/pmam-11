using Infrastructure.Models.User;

namespace Infrastructure.Models.Order;

public class OrderStatusHistoryModel
{
	public long Id { get; set; }
	public string StatusName { get; set; }
	public UserModel ChangedBy { get; set; }
	public DateTimeOffset TimeOfChange { get; set; }
}
