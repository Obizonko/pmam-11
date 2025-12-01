using Domain;
using Infrastructure.Models.Menu;

namespace Infrastructure.Interfaces;

public interface IMenuService
{
    void AddMealToMenu(string name, int categoryId, double price, double? weight, string imageUrl, string description);
    void DeleteMealFromMenu(int id);
    IList<CategoryItemModel> GetCategories();
    MealModel GetMealById(int id);
    IList<MealModel> GetMeals(int categoryId);
    void UpdateMeal(long id, string name, int categoryId, double price, double? weight, string? imageUrl, string? description);
}
