using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FODLSystem.Models
{
    public class FuelOil
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Display(Name = "Reference No")]
        public string ReferenceNo { get; set; }
        public string Shift { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }

       // [Required]
        [Display(Name = "Dispenser")]
        //public int DispenserId { get; set; }
        [ForeignKey("Dispensers")]
        [Column(TypeName = "VARCHAR(20)")]
        public string DispenserCode { get; set; }
        public virtual Dispenser Dispensers { get; set; }

       // [Required]
        [Display(Name = "Lube Truck")]
        //public int LubeTruckId { get; set; }
        [ForeignKey("LubeTrucks")]
        [Column(TypeName = "VARCHAR(20)")]
        public string LubeTruckCode { get; set; }
        public virtual LubeTruck LubeTrucks { get; set; }

        public DateTime TransactionDate { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime TransferDate { get; set; }
        public string TransferredBy { get; set; }
        public int OldId { get; set; }
        public string SourceReferenceNo { get; set; }
        public DateTime OriginalDate { get; set; }
        public string BatchName { get; set; }
        public string DispenserName { get; set; }
        public bool GeneratedfromBSmart { get; set; }
        public virtual List<FuelOilDetail> FuelOilDetail { get; set; }

}
}
