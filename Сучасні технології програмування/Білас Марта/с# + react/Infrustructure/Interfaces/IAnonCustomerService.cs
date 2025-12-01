
namespace Infrastructure.Interfaces;

public interface IAnonCustomerService
{
    Guid CreateAnonCustomer();
    Guid? GetAnonCustomer();
}
