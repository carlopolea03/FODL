using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FODLSystem.Models
{
    public class Dispenser
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Key]
        [Column(TypeName = "VARCHAR(20)")]
        [Required]
        public string No { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        [Required]
        public string Name { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        [Required]
        public string NewName { get; set; }

        public string Status { get; set; } = "Active";
        public DateTime? DateModified { get; set; }



        [ForeignKey("Location")]
        [Column(TypeName = "VARCHAR(20)")]
        public string LocationCode { get; set; }
        public virtual Location Location { get; set; }
    }
}
