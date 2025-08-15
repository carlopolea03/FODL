﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FODLSystem.Models
{
    public class Location
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Key]
        [Column(TypeName = "VARCHAR(20)")]
        [Required]
        public string No { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        [Required]
        public string List { get; set; }
        [Display(Name = "Office Code")]
        [Column(TypeName = "VARCHAR(10)")]
        [Required]
        public string OfficeCode { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime? DateModified { get; set; } = DateTime.Now;
    }
}
