namespace Infrastructure.Interfaces;

public interface IMealImportService
{
    bool ImportMeals(string jsonSource);
}
