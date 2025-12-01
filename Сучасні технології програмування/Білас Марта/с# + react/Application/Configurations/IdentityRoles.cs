namespace Application.Configurations;

public static class IdentityRoles
{
	public const string SuperAdmin = "SuperAdmin";
	public const string Admin = "Admin";
	public const string User = "User";

	public static List<string> GetRoles()
	{
		return typeof(IdentityRoles).GetFields()
			   .Where(field => field.IsLiteral && !field.IsInitOnly)
			   .Select(field => field.GetValue(null).ToString())
			   .ToList();
	}
}
