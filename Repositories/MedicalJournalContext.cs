using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Entites;
using System.Data.Entity.ModelConfiguration.Conventions;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Repositories
{    
    public class MedicalJournalContext : IdentityDbContext<ApplicationUser>
    {
        public MedicalJournalContext()
            : base("MedicalJournalContext",throwIfV1Schema:false)
        {
            Database.SetInitializer<MedicalJournalContext>(new MedicalJournalInitializer());
        }

        //static MedicalJournalContext()
        //{
        //    Database.SetInitializer<MedicalJournalContext>(new MedicalJournalInitializer());
        //}

        public DbSet<Journal> Journals { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<Client> Clients { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        //}

        public static MedicalJournalContext Create()
        {
            return new MedicalJournalContext();
        }
    }

    //public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    //{
    //    public ApplicationDbContext()
    //        : base("MedicalJournalContext", throwIfV1Schema: false)
    //    {
    //    }

    //    public static ApplicationDbContext Create()
    //    {
    //        return new ApplicationDbContext();
    //    }
    //}

}
