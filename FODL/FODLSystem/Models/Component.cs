using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FODLSystem.Models
{
    public class Component
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Key]
        [Column(TypeName = "VARCHAR(20)")]
        [Required]
        public string Code { get; set; }

        [Column(TypeName = "VARCHAR(20)")]
        public string Description { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime DateModified { get; set; }
    }
}
