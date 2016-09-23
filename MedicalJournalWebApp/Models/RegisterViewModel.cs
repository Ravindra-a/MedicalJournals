using Business.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MedicalJournalWebApp.Models
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string RegisterFullName { get; set; }

        [Required]
        [EmailAddress]
        public string RegisterEmail { get; set; }

        [Required]
        [Phone]
        [RegularExpression(@"^[789]\d{9}$", ErrorMessage = "Not a valid Phone number")]
        public string RegisterPhoneNumber { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string RegisterPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("RegisterPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string RegisterConfirmPassword { get; set; }

        public bool IsPublisher { get; set; }

        [Required]
        public ApplicationRoles SelectedRole { get; set; }

    }

   
}