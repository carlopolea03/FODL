﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FODLSystem.Models
{
    public enum Domain
    {
       SEMIRARAMINING, SMCDACON
    }


    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column(TypeName = "VARCHAR(50)")]

        public string Username { get; set; }
        public int RoleId { get; set; }
        
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public virtual Role Roles { get; set; }
        public string Email { get; set; }
       
        public string Domain { get; set; }
        public string CompanyAccess { get; set; }
        public string UserType { get; set; }

        //public int DepartmentId { get; set; }

        [ForeignKey("Departments")]
        [Column(TypeName = "VARCHAR(20)")]
        public string DepartmentCode { get; set; }
        public virtual Department Departments { get; set; }

        public string LubeAccess { get; set; }
        public string DispenserAccess { get; set; }
        public DateTime? DateModified { get; set; } = DateTime.Now;

    }


}
