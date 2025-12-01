namespace Infrastructure.Models.User;

public class UpdateUserPasswordModel
{
    public string Email { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}
