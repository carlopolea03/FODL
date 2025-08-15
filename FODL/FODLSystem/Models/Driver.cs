using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FODLSystem.Models
{
    public class Driver
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Key]
        [Required]
        [Column(TypeName = "VARCHAR(50)")]
        public string IdNumber { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        public string Position { get; set; }
        public string Status { get; set; }
        public DateTime? DateModified { get; set; } =DateTime.Now;
    }
}
