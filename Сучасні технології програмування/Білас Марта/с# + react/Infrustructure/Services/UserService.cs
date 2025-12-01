using DataContext;
using Domain;
using Domain.Idenity;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Infrastructure.Models.Order;
using Infrastructure.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class UserService : IUserService
{
	private readonly UserManager<AppUser> _userManager;
	private readonly SignInManager<AppUser> _signInManager;
	private readonly RoleManager<AppRole> _roleManager;
	private readonly RestaurantDataContext _db;

	public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, RestaurantDataContext db)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_roleManager = roleManager;
		_db = db;
	}
	public async Task<IdentityResult> CreateUserAsync(string address, string email, string firstName, string lastName, string phoneNumber, string password, string roleName)
	{
		var existingUser = await _userManager.FindByEmailAsync(email);
		if (existingUser != null)
		{
			return IdentityResult.Failed(new IdentityError { Description = "There is user with such email." });
		}
		var user = new AppUser
		{
			Address = address,
			Email = email,
			FirstName = firstName,
			LastName = lastName,
			PhoneNumber = phoneNumber,
			EmailConfirmed = true,
			UserName = email,
		};
		var result = await _userManager.CreateAsync(user, password);
		if (result.Succeeded)
		{
			var roleExists = await _roleManager.RoleExistsAsync(roleName);
			if (roleExists)
			{
				await _userManager.AddToRoleAsync(user, roleName);
			}
		}
		return result;
	}

	public async Task<(UserModel, string)> SignInAsync(string email, string password)
	{
		var user = await _userManager.FindByEmailAsync(email);

		if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
		{
			return (null, "There is no such user.");
		}

		var rememberMe = true;
		var role = await _userManager.GetRolesAsync(user);

		var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: false);
		if (result.Succeeded)
			return (new UserModel
			{
				Email = user.Email,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Address = user.Address,
				PhoneNumber = user.PhoneNumber,
				Role = role
			}, "Success");
		else
			return (null, "Email or password is not correct.");

	}

	public async Task<bool> LogOutAsync()
	{
		try
		{
			await _signInManager.SignOutAsync();
			return true;
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}
	public async Task<IdentityResult> DeleteUserAsync(string email)
	{
		var user = await _userManager.FindByEmailAsync(email);
		if (user == null)
		{
			return IdentityResult.Failed(new IdentityError { Description = $"User with email {email} does not exist." });
		}

		var result = await _userManager.DeleteAsync(user);
		return result;
	}
	public async Task<IdentityResult> UpdateUserInfoAsync(string email, string? newEmail = null, string? newPhoneNumber = null, string? newFirstName = null, string? newLastName = null, string? newAddress = null)
	{
		var user = await _userManager.FindByEmailAsync(email);
		if (user == null)
		{
			return IdentityResult.Failed(new IdentityError { Description = $"User with email {email} does not exist." });
		}

		if (!string.IsNullOrEmpty(newEmail))
		{
			user.Email = newEmail;
		}

		if (!string.IsNullOrEmpty(newPhoneNumber))
		{
			user.PhoneNumber = newPhoneNumber;
		}

		if (!string.IsNullOrEmpty(newFirstName))
		{
			user.FirstName = newFirstName;
		}

		if (!string.IsNullOrEmpty(newLastName))
		{
			user.LastName = newLastName;
		}

		if (!string.IsNullOrEmpty(newAddress))
		{
			user.Address = newAddress;
		}

		var result = await _userManager.UpdateAsync(user);
		return result;
	}

	public async Task<IdentityResult> UpdateUserPasswordAsync(string email, string oldPassword, string newPassword)
	{
		var user = await _userManager.FindByEmailAsync(email);
		if (user == null)
		{
			return IdentityResult.Failed(new IdentityError { Description = $"User with email {email} does not exist." });
		}

		var verifyPassword = await _userManager.CheckPasswordAsync(user, oldPassword);
		if (!verifyPassword)
		{
			return IdentityResult.Failed(new IdentityError { Description = "Old password is not correct." });
		}

		var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
		return result;
	}

	public async Task<UserOrdersModel> GetUserOrdersByEmail(string email)
	{
		var user = await _userManager.FindByEmailAsync(email);

		var orders = _db.Orders?
			.Include(o => o.OrderRows)
				.ThenInclude(o => o.Meal)
			.Include(o => o.OrderStatusHistories)
				.ThenInclude(osh => osh.OrderStatus)
			.Where(o => o.Customer.Email == email && o.IsPaid == true)
			.OrderBy(o => o.Id)
			.ToList();

		if (orders.Count() == 0)
			return new UserOrdersModel();

		var userOrders = new UserOrdersModel
		{
			Orders = orders.Select(o =>
			{
				var lastStatusHistory = o.OrderStatusHistories
					.OrderByDescending(os => os.TimeOfChange)
					.FirstOrDefault();

				return new OrderModel
				{
					Id = o.Id,
					OrderRows = o.OrderRows.Select(or => new OrderRowModel
					{
						Id = or.Id,
						MealName = or.Meal.Name,
						Weight = or.Meal.Weight,
						ImageUrl = or.Meal.ImageUrl,
						Price = or.Price,
						Amount = or.Amount
					}),
					OrderStatus = lastStatusHistory != null ? new OrderStatusModel
					{
						Id = lastStatusHistory.OrderStatus.Id,
						Name = lastStatusHistory.OrderStatus.Name
					} : new OrderStatusModel
					{
						Id = 0,
						Name = "Unknown"
					}
				};
			}).ToList()
		};

		return userOrders;
	}
}


