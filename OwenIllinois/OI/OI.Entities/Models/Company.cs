using System.ComponentModel.DataAnnotations;
using Repository.Pattern.Ef6;

namespace OI.Entities.Models
{
    public class Company : Entity
    {
        [Key]
        public int Id { get; set; }
        [Required]

        [MaxLength(250)]
        public string CorporateName { get; set; }
    }
}
