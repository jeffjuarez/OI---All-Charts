using System.ComponentModel.DataAnnotations;
using Repository.Pattern.Ef6;

namespace OI.Entities.Models
{
    public class Account : Entity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Username { get; set; }

        [Required]
        [MaxLength(32)]
        public string Password { get; set; }        
        
        public string Salt { get; set; }

        public bool IsResetPassword { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int RoleId { get; set; }

        // Navigation properties
        public virtual Employee Employee { get; set; }

        public virtual Role Role { get; set; }
    }
}
