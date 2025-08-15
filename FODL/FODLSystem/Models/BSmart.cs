using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FODLSystem.Models
{
    public class BSmartShift
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string ReferenceNo { get; set; }
        public string Shift { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        [ForeignKey("Dispensers")]
        public string DispenserCode { get; set; }
        public string DispenserName { get; set; }
        public virtual Dispenser Dispensers { get; set; }

        [ForeignKey("LubeTrucks")]
        public string LubeTruckCode { get; set; }
        public virtual LubeTruck LubeTrucks { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; } 
        public DateTime TransferDate { get; set; }
        public string TransferredBy { get; set; }
        public string SourceReferenceNo { get; set; }
        public DateTime OriginalDate { get; set; }
        public string BatchName { get; set; }
        public int FuelOilId { get; set; }
        public int OldId { get; set; }
        public string OriginalReferenceNo { get; set; }
        public List<BSmartEquipment> BSmartEquipment { get; set; }
    }
    public class BSmartEquipment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Equipments")]
        public string EquipmentNo { get; set; }
        public virtual Equipment Equipments { get; set; }
        [ForeignKey("Locations")]
        public string LocationNo { get; set; }
        public virtual Location Locations { get; set; }
        public decimal? SMR { get; set; }

        public string Signature { get; set; }
        public string Status { get; set; } 
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        public int FuelOilId { get; set; }
        public int BSmartShiftId { get; set; }
        public virtual BSmartShift BSmartShift { get; set; }
        public int OldId { get; set; }
        [ForeignKey("Drivers")]
        public string DriverIdNumber { get; set; }
        public virtual Driver Drivers { get; set; }
        [Display(Name = "Detail No")]
        public string DetailNo { get; set; }

        [Display(Name = "Old Detail No")]
        public string OldDetailNo { get; set; }
        public DateTime TransferredDate { get; set; } = DateTime.Now;
        public List<BSmartItem> BSmartItems { get; set; }

    }
    public class BSmartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime TimeInput { get; set; }
        [ForeignKey("Items")]
        public string ItemNo { get; set; }
        public virtual Item Items { get; set; }
        [ForeignKey("Components")]
        public string ComponentCode { get; set; }
        public virtual Component Components { get; set; }
        public decimal VolumeQty { get; set; }
        public int BSmartEquipmentId { get; set; }
        public virtual BSmartEquipment BSmartEquipment { get; set; }
        public string Status { get; set; } = "Active";
        public int OldId { get; set; }
        public string OldFuelOilDetailNo { get; set; }
        public DateTime TransferredDate { get; set; } = DateTime.Now;

    }
    public class BSmartError
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime ErrorDate { get; set; }
        public string ErrMessage { get; set; }
    }

    #region VIEWMODEL
    public class Shift
    {
        public int apiMidShiftId { get; set; }
        public DateTime apiMidShiftDatetime { get; set; }
        public string referenceNo { get; set; }
        public string shift { get; set; }
        public DateTime createdDate { get; set; }
        public string createdBy { get; set; }
        public string status { get; set; }
        public string dispenserCode { get; set; }
        public string lubeTruckCode { get; set; }
        public DateTime transactionDate { get; set; }
        public int id { get; set; }
        public DateTime originalDate { get; set; }

    }
    class BSItem
    {
        public string apiMidItemId { get; set; }
        public DateTime? apiMidItemDatetime { get; set; }
        public DateTime timeInput { get; set; }
        public string itemNo { get; set; }
        public string componentCode { get; set; }
        public decimal quantity { get; set; }
        public string detailNo { get; set; }
        public string status { get; set; }
        public int id { get; set; }
        public string createdBy { get; set; }
        public string referenceNo { get; set; }
    }
    public class BSEquipment
    {
        public string apiMidEquipmentId { get; set; }
        public DateTime? apiMidEquipmentDatetime { get; set; }
        public DateTime? createdDate { get; set; }
        public string equipmentNo { get; set; }
        public string locationNo { get; set; }
        public string referenceNo { get; set; }
        public string status { get; set; }
        public decimal? smr { get; set; }
        public string signature { get; set; }
        public int id { get; set; }
        public string createdBy { get; set; }
        public string driverIdNumber { get; set; }
        public string detailNo { get; set; }
        public string apiSrcEquipmentId { get; set; }
    }
    #endregion
}
