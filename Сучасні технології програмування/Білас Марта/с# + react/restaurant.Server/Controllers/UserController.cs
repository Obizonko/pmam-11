using Application.Settings;
using Domain.Idenity;
using Infrastructure.Interfaces;
using Infrastructure.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace restaurant.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{

	public readonly IUserService _userService;
	public readonly UserManager<AppUser> _userManager;
	public readonly JwtSettings _jwtSettings;

	public UserController(IUserService userService, UserManager<AppUser> userManager, IOptions<JwtSettings> jwtSettings)
	{
		_userService = userService;
		_userManager = userManager;
		_jwtSettings = jwtSettings.Value;
	}

	[HttpPost("signUp")]
	public async Task<IActionResult> CreateUser(CreateUserModel model)
	{
		var result = await _userService.CreateUserAsync(model.Address, model.Email, model.FirstName, model.LastName, model.PhoneNumber, model.Password, "user");
		if (result.Succeeded)
			return Ok(result);
		else
			return BadRequest(result);
	}

	[HttpPost("signIn")]
	public async Task<IActionResult> SignIn(SignInModel model)
	{
		var (user, message) = await _userService.SignInAsync(model.Email, model.Password);
		if (user != null)
			return Ok(user);
		else
			return Ok(new { message = message });
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] SignInModel model)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var user = await _userManager.FindByEmailAsync(model.Email);
		if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
		{
			// Create user claims including roles.
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.UserName),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
			};

			var roles = await _userManager.GetRolesAsync(user);
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _jwtSettings.Issuer,
				audience: _jwtSettings.Audience,
				claims: claims,
				expires: DateTime.Now.AddHours(1),
				signingCredentials: creds);

			return Ok(new
			{
				token = new JwtSecurityTokenHandler().WriteToken(token)
			});
		}

		return Unauthorized();
	}

	[HttpGet("logOut")]
	public async Task<IActionResult> LogOut()
	{
		try
		{
			var result = await _userService.LogOutAsync();

			if (result)
				return Ok(new { message = "You have been successfully logged out." });
			else
				return StatusCode(500, new { error = "Logout failed." });

		}
		catch (Exception ex)
		{
			return StatusCode(500, new { error = "An unexpected error occurred." });
		}
	}

	[HttpPut("UpdateUserInfo")]
	public async Task<IActionResult> UpdateUserInfoAsync([FromBody] UpdateUserInfoModel newInfo)
	{
		var result = await _userService.UpdateUserInfoAsync(newInfo.Email, newInfo.NewEmail, newInfo.PhoneNumber, newInfo.FirstName, newInfo.LastName, newInfo.Address);

		if (result.Succeeded)
		{
			return Ok();
		}

		return BadRequest(result.Errors);
	}

	[HttpPut("UpdateUserPassword")]
	public async Task<IActionResult> UpdatePasswordAsync([FromBody] UpdateUserPasswordModel model)
	{
		IdentityResult result = await _userService.UpdateUserPasswordAsync(model.Email, model.OldPassword, model.NewPassword);

		if (result.Succeeded)
		{
			return Ok();
		}

		return BadRequest(result.Errors);
	}
}
