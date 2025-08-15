using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FODLSystem.Models
{
    public class FuelOilSubDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime TimeInput { get; set; }

        //public int ItemId { get; set; }

        [Display(Name = "Item No")]
        [ForeignKey("Items")]
        [Column(TypeName = "VARCHAR(20)")]
        public string ItemNo { get; set; }
        public virtual Item Items { get; set; }

        //public int ComponentId { get; set; }

        [Display(Name = "Component")]
        [ForeignKey("Components")]
        [Column(TypeName = "VARCHAR(20)")]
        public string ComponentCode { get; set; }

        public virtual Component Components { get; set; }


        public decimal VolumeQty { get; set; }
        public int FuelOilDetailId { get; set; }
        public virtual FuelOilDetail FuelOilDetails { get; set; }
        public string Status { get; set; } = "Active";
        public int OldId { get; set; }

        [Display(Name = "Old Fuel Oil Detail No")]
        public string OldFuelOilDetailNo { get; set; }

    }
}
