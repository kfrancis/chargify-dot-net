using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ChargifySampleSite.MVC3.Models
{
    #region Models

    public class DirectSubscriptionViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Question { get; set; }

        [Required]
        public string Answer { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "CC Number")]
        public string CardNumber { get; set; }

        public string CVV { get; set; }

        [Display(Name = "Exp. Month")]
        public int ExpMonth { get; set; }

        [Display(Name = "Exp. Year")]
        public int ExpYear { get; set; }
    }

    public class RemoteSubscriptionViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Question { get; set; }

        [Required]
        public string Answer { get; set; }

        [Required]
        public string Email { get; set; }
    }

    public class LocalSubscriptionViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Question { get; set; }

        [Required]
        public string Answer { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }

        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Display(Name = "CC Number")]
        public string CardNumber { get; set; }

        public string CVV { get; set; }

        [Display(Name = "Exp. Month")]
        public int ExpMonth { get; set; }

        [Display(Name = "Exp. Year")]
        public int ExpYear { get; set; }
    }

    #endregion

}