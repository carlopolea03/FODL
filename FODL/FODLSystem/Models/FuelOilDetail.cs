using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FODLSystem.Models
{
    public class FuelOilDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        //[Display(Name = "Unit No")]

        // public int EquipmentId { get; set; }

        [ForeignKey("Equipments")]
        [Column(TypeName = "VARCHAR(50)")]
        public string EquipmentNo { get; set; }
        public virtual Equipment Equipments { get; set; }

        [ForeignKey("Locations")]
        [Display(Name = "Location")]
      //  public int LocationId { get; set; }
        public string LocationNo { get; set; }
        public virtual Location Locations { get; set; }


        public decimal? SMR { get; set; }

        public string Signature { get; set; }
        public string Status { get; set; } = "Active";
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        public int FuelOilId { get; set; }
        public virtual FuelOil FuelOils { get; set; }
        public int OldId { get; set; }

        //public int DriverId { get; set; }
        [ForeignKey("Drivers")]
        public string DriverIdNumber { get; set; }
        public virtual Driver Drivers { get; set; }

        [Display(Name = "Detail No")]
        public string DetailNo { get; set; }

        [Display(Name = "Old Detail No")]
        public string OldDetailNo { get; set; }


        [Display(Name = "Job Card No.")]
        public string JobCardNo { get; set; }
        public virtual List<FuelOilSubDetail> FuelOilSubDetail { get; set; }
    }
    public class JobCardViewModel
    {
        public string text { get; set; }
        public string id { get; set; }
        public string title { get; set; }
    }
}
