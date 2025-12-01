using Infrastructure.Models.User;

namespace Infrastructure.Models.Order;

public class OrderWithStatusHistoryModel
{
	public long Id { get; set; }
	public IEnumerable<OrderRowModel>? OrderRows { get; set; }
	public UserModel? Customer { get; set; }
	public double Total { get; set; }
	public List<OrderStatusHistoryModel> StatusHistory { get; set; }
	public DateTime OrderDate { get; set; }
}
