using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models.Order;

public class PlaceOrderModel
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Surname { get; set; }

    [Required]
    public string Phone { get; set; }

    [Required]
    public string Address { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public int PaymentType { get; set; }
    public string? AdditionalInfo { get; set; }
}
