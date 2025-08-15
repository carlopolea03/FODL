using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FODLSystem.Models
{
    public class LubeTruck
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Key]
        [Column(TypeName = "VARCHAR(20)")]
        [Required]
        public string No { get; set; }

        public string OldId { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime? DateModified { get; set; } = DateTime.Now;

        //[ForeignKey("Location")]
        [Column(TypeName = "VARCHAR(20)")]
        public string LocationCode { get; set; }
      //  public virtual Location Location { get; set; }
    }
}
