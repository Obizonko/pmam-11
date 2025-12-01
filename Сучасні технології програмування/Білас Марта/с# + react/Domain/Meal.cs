namespace Domain;

public class Meal
{
    public long Id { get; set; }
    public string Name { get; set; }
    public virtual MealCategory Category { get; set; }
    public double Price { get; set; }
    public double? Weight { get; set; }
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }
}
