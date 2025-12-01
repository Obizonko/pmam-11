namespace Domain;

public class OrderRow
{
    public long Id { get; set; }
    public virtual Meal Meal { get; set; }
    public int Amount { get; set; }
    public double Price { get; set; }
    public double Total
    {
        get
        {
            return Price * Amount;
        }
    }
}
