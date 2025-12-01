namespace Infrastructure.Models.Order;

public class OrderRowModel
{
    public long Id { get; set; }
    public string MealName { get; set; }
    public double? Weight { get; set; }
    public string ImageUrl { get; set; }
    public double Price { get; set; }
    public int Amount { get; set; }
    public double Sum
    {
        get
        {
            return Price * Amount;
        }
    }
}
