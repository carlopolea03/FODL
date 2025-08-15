﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FODLSystem.Models.View_Model
{
    public class UserViewModel
    {

        public string Username { get; set; }
        public int id { get; set; }
        public string Roles { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Name { get; set; }
        public string sysid { get; set; }
        public string status { get; set; }
        public string mail { get; set; }
        public string domain { get; set; }
        public UserViewModel() { }
        public UserViewModel(UserViewModel i)
        {
            id = i.id;
            Username = i.Username;
            Roles = i.Roles;
            Lastname = i.Lastname;
            Firstname = i.Firstname;
            Name = i.Name;
            sysid = i.sysid;
            status = i.status;
            mail = i.mail;
            domain = i.domain;
        }

    }


    public class LocalUserViewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Domain")]
        public string Domain { get; set; }
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Roles")]
        public int RoleId { get; set; }
        [Required]
        public string Lastname { get; set; }
        [Required]
        public string Firstname { get; set; }

        public int DepartmentId { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string Status { get; set; }
        public string DepartmentCode { get; set; }
    }

    public class DomainUserViewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Domain")]
        public string Domain { get; set; }
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Roles")]
        public int RoleId { get; set; }
        [Required]
        public string Lastname { get; set; }
        [Required]
        public string Firstname { get; set; }

        public int DepartmentId { get; set; }

        public string Status { get; set; }
        public string DepartmentCode { get; set; }
    }


    public class LoginViewModel
    {


        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }
        //[Required]
        [Display(Name = "Domain")]
        public string Domain { get; set; }
        //[Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        public string LogInType { get; set; }
    }
}
