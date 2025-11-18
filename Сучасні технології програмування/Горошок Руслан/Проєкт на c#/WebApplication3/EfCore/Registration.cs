using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication3.EfCore
{
    [Table("Registration")]
    public class Registration
    {
        [Key,Required]
        public int ID { get; set; }
        public string UserName { get; set; }

        public string PASS { get; set; }

        public string Email { get; set; }

        public string IsActive { get; set; }

    }
}
 