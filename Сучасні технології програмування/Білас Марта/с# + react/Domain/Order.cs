using Domain.Enums;
using Domain.Idenity;
using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Order
{
	[Key]
	public long Id { get; set; }

	public Guid AnonId { get; set; }
	public virtual ICollection<OrderRow>? OrderRows { get; set; }
	public virtual AppUser? Customer { get; set; }
	public bool IsPaid { get; set; }
	public PaymentType PaymentType { get; set; }
	public double Total { get; set; }
	public DateTime OrderDate { get; set; }
	public string? AdditionalInfo { get; set; }

	public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new HashSet<OrderStatusHistory>();

	public void RecalculateTotal()
	{
		Total = OrderRows?.Sum(x => x.Price * x.Amount) ?? 0;
	}
}
