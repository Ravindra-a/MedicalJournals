using Business.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Business.Entites.Auth
{
    [DataContract]
    public class ProfileViewModel
    {
        [DataMember]
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string RegisteredEmail { get; set; }

        [DataMember]
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^[789]\d{9}$", ErrorMessage = "Not a valid Phone number")]
        public string RegisteredPhoneNumber { get; set; }

        [DataMember]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [Display(Name = "Name")]
        public string ProfileName { get; set; }
        public string Url { get; set; }
        public string ProfileId { get; set; }
        public IList<string> ProfileRoles { get; set; }
        public IList<System.Security.Claims.Claim> ProfileClaims { get; set; }        
        public DateTime CreatedTime { get; set; }
    }
    
}