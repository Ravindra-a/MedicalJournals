using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Business.Entites.Auth
{
    [DataContract]
    public class RegisterViewModel
    {
        [DataMember]
        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string RegisterFullName { get; set; }

        [Required]
        [EmailAddress]
        [DataMember]
        public string RegisterEmail { get; set; }

        [Required]
        [Phone]
        [RegularExpression(@"^[789]\d{9}$", ErrorMessage = "Not a valid Phone number")]
        [DataMember]
        public string RegisterPhoneNumber { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [DataMember]
        public string RegisterPassword { get; set; }

        [Required]
        [DataMember]
        public ApplicationRoles SelectedRole { get; set; }

    }

    [DataContract]
    public class ResetPasswordViewModel
    {
        [DataMember]
        [Required]
        [EmailAddress]
        public string ResetPasswordEmail { get; set; }

        [DataMember]
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string ResetPasswordPassword { get; set; }

        [DataMember]
        [DataType(DataType.Password)]
        [Compare("ResetPasswordPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ResetPasswordConfirmPassword { get; set; }

        [DataMember]
        public string ResetPasswordCode { get; set; }
    }

    [DataContract]
    public class ForgotPasswordViewModel
    {
        [DataMember]
        [Required]
        [EmailAddress]
        public string ForgotPasswordEmail { get; set; }
        [DataMember]
        public string ForgotPasswordClientURL { get; set; }
    }

    [DataContract]
    public class ChangePasswordViewModel
    {
        [DataMember]
        [Required]
        [DataType(DataType.Password)]        
        public string OldPassword { get; set; }

        [DataMember]
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataMember]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
