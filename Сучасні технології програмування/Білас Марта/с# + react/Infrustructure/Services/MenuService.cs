using DataContext;
using Domain;
using Infrastructure.Interfaces;
using Infrastructure.Models.Menu;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class MenuService : IMenuService
{
    public readonly RestaurantDataContext _db;
    public MenuService(RestaurantDataContext db)
    { _db = db; }

    public IList<MealModel> GetMeals(int categoryId)
    {
        var meals = _db.Meals
            .Include(m => m.Category)
            .Where(m => m.Category.Id == categoryId)
            .ToList();

        var result = meals.Select(meal => new MealModel
        {
            Id = meal.Id,
            Name = meal.Name,
            CategoryId = meal.Category.Id,
            Price = meal.Price,
            Weight = meal.Weight,
            ImageUrl = meal.ImageUrl,
            Description = meal.Description
        }).ToList();

        return result;
    }

    public MealModel GetMealById(int id)
    {
        var meal = _db.Meals
            .Include(m => m.Category)
            .FirstOrDefault(m => m.Id == id);
        if (meal == null)
        {
            return new MealModel();
        }

        return new MealModel
        {
            Id = meal.Id,
            Name = meal.Name,
            CategoryId = meal.Category.Id,
            Price = meal.Price,
            Weight = meal.Weight,
            ImageUrl = meal.ImageUrl,
            Description = meal.Description
        };
    }

    public IList<CategoryItemModel> GetCategories()
    {
        IList<CategoryItemModel> categories = _db.MealCategories
                .Select(category => new CategoryItemModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    ImageUrl = category.ImageUrl
                })
                .ToList();


        return categories;
    }

    public void AddMealToMenu(string name, int categoryId, double price, double? weight, string imageUrl, string description)
    {
        var category = _db.MealCategories.Where(c => c.Id == categoryId).FirstOrDefault();
        if (category == null)
        {
            throw new Exception("there is no such category");
        }

        var newMeal = new Meal
        {
            Name = name,
            Category = category,
            Price = price,
            Weight = weight,
            ImageUrl = imageUrl,
            Description = description
        };

        _db.Meals.Add(newMeal);

    }

    public void DeleteMealFromMenu(int id)
    {
        var meal = _db.Meals.FirstOrDefault(c => c.Id == id);
        if (meal == null)
        {
            throw new Exception("there is no such meal");
        }

        _db.Meals.Remove(meal);
    }

    public void UpdateMeal(long id, string name, int categoryId, double price, double? weight, string? imageUrl, string? description) 
    { 
        var meal = _db.Meals.FirstOrDefault(c => c.Id == id);

        if (meal == null)
        {
            throw new Exception("there is no such meal");
        }
        meal.Weight = weight;
        meal.ImageUrl = imageUrl;
        meal.Description = description;
        meal.Name = name;

        _db.SaveChanges();
    }

}
