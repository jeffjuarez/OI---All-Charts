using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Repository.Pattern.Ef6;

namespace OI.Entities.Models
{
    public class Employee : Entity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string EmployeeNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string Fullname { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(1)]
        public string Gender { get; set; }

        [Required]
        [MaxLength(25)]
        public string MemberType { get; set; }

        public string Remarks { get; set; }

        [Required]
        public int CompanyId { get; set; }

        // Navigation properties
        public virtual Company Company { get; set; }

        public virtual ICollection<Document> Documents { get; set; }

        [NotMapped]
        public virtual Account Account { get; set; }
    }
}
