namespace Infrastructure.Models;

public class ImportMealModel
{
    public long Id { get; set; }
    public string Name { get; set; }
    public int CategoryId { get; set; }
    public double Price { get; set; }
    public double? Weight { get; set; }
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }
}
