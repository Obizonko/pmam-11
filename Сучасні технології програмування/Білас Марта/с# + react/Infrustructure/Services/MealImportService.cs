using DataContext;
using Domain;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Services;

public class MealImportService: IMealImportService
{
    private readonly RestaurantDataContext _db;

    public MealImportService(RestaurantDataContext db)
    {
        _db = db;
    }

    public bool ImportMeals(string jsonSource)
    {
        var importMeals = JsonConvert.DeserializeObject<IList<ImportMealModel>>(jsonSource);
        var meals = importMeals.Select(importMeal => new Meal
        {
            Id = importMeal.Id,
            Name = importMeal.Name,
            Category = _db.MealCategories.FirstOrDefault(c => c.Id == importMeal.CategoryId),
            Price = importMeal.Price,
            Weight = importMeal.Weight,
            ImageUrl = importMeal.ImageUrl,
            Description = importMeal.Description
        }).ToList();
        _db.Meals.AddRange(meals);
        _db.SaveChanges();

        return true;
    }
}
