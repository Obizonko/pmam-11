namespace Domain;

public class OrderStatus
{
	public long Id { get; set; }
	public string? Name { get; set; }

	public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new HashSet<OrderStatusHistory>();
}
