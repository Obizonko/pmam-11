using DataContext;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class AnonCustomerService: IAnonCustomerService
{
    private readonly RestaurantDataContext _db;
    private readonly IHttpContextAccessor _contextAccessor;

    public AnonCustomerService(RestaurantDataContext db, IHttpContextAccessor contextAccessor)
    {
        _db = db;
        _contextAccessor = contextAccessor;
    }

    public Guid CreateAnonCustomer()
    {
        var anonId = _contextAccessor.HttpContext.Request.Cookies["anonimusId"];

        if (anonId == null)
        {
            anonId = Guid.NewGuid().ToString();
            _contextAccessor.HttpContext.Response.Cookies.Append("anonimusId", anonId.ToString());
        }

        return new Guid(anonId);
    }

    public Guid? GetAnonCustomer()
    {
        var anonId = _contextAccessor.HttpContext.Request.Cookies["anonimusId"];

        if (anonId == null)
        {
            return null;
        }

        return new Guid(anonId);
    }

}
