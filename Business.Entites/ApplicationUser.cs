using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using Microsoft.AspNet.Identity;

namespace Business.Entites
{
    public class ApplicationUser : IdentityUser
    {

        public string ProfileName { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

        public virtual ICollection<Journal> Journals { get; set; }

        public virtual ICollection<UserSubscription> UserSubscriptions { get; set; }
    }

    public enum ApplicationRoles
    {
        Subscriber,
        Publisher        
    }
}
