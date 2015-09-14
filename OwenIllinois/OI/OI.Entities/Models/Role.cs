using System.ComponentModel.DataAnnotations;
using Repository.Pattern.Ef6;

namespace OI.Entities.Models
{
    public class Role : Entity
    {
        [Key]
        public int Id { get; set; }
        [Required]

        [MaxLength(30)]
        public string RoleType { get; set; }
    }
}
