using FODLSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FODLSystem.Extension
{
    public class ValidateJO : ValidationAttribute
    {
        //protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        //{
        //    var documentEntry = (FuelOilDetail)validationContext.ObjectInstance;
            
        //    string[] disp ={ "PMS-NRR", "PMS-MWS", "PMS-MTL", "PMS-MLV"};
        //    if (disp.Contains(documentEntry.))
        //    {
        //        return new ValidationResult("Invoice amount must have a value.");
        //    }
        //    return ValidationResult.Success;
        //}
    }

    public class Select2Model
    {
        public string text { get; set; }
        public string id { get; set; }
        public string title { get; set; }
    }
}
