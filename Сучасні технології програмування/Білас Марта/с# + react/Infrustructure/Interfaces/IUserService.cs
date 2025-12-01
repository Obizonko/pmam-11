using Infrastructure.Models.User;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Interfaces;

public interface IUserService
{
    Task<IdentityResult> CreateUserAsync(string address, string email, string firstName, string lastName, string phoneNumber, string password, string roleName);
    Task<UserOrdersModel> GetUserOrdersByEmail(string email);
    Task<bool> LogOutAsync();
    Task<(UserModel, string)> SignInAsync(string email, string password);
    Task<IdentityResult> UpdateUserInfoAsync(string email, string? newEmail = null, string? newPhoneNumber = null, string? newFirstName = null, string? newLastName = null, string? newAddress = null);
    Task<IdentityResult> UpdateUserPasswordAsync(string email, string oldPassword, string newPassword);
}
