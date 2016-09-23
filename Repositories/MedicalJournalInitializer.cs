using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Entites;
using System.Data.Entity;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Net.Http;
using System.Net;
using Microsoft.Owin.Security;
using Business.Entites.Auth;

namespace Repositories
{
    public class MedicalJournalInitializer : DropCreateDatabaseIfModelChanges<MedicalJournalContext>
    {
        protected override void Seed(MedicalJournalContext context)
        {

            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();

            //Create Role publisher if it does not exist
            string roleEnum = ApplicationRoles.Publisher.ToString();
            var role = roleManager.FindByName(roleEnum);
            if (role == null)
            {
                role = new ApplicationRole(roleEnum);
                var roleresult = roleManager.Create(role);
            }

            //Create Role subscriber
            roleEnum = ApplicationRoles.Subscriber.ToString();
            role = roleManager.FindByName(roleEnum);
            if (role == null)
            {
                role = new ApplicationRole(roleEnum);
                var roleresult = roleManager.Create(role);
            }

        }
    }
}
