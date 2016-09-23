using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Entites;

namespace Repositories.Repositories
{
    public class UserSubscriptionRepository : IUserSubscription
    {
        MedicalJournalContext context = new MedicalJournalContext();

        public void Add(UserSubscription userSubscription)
        {
            context.UserSubscriptions.Add(userSubscription);
            context.SaveChanges();
        }

        public void Delete(int userSubscriptionId)
        {
            UserSubscription u = context.UserSubscriptions.Find(userSubscriptionId);
            if(u != null)
            {
                context.UserSubscriptions.Remove(u);
                context.SaveChanges();
            }
        }

        public List<UserSubscription> GetUserSubscriptions()
        {
            return context.UserSubscriptions.ToList();
        }

        public UserSubscription GetSubscriptionById(int userSubscriptionId)
        {
            return context.UserSubscriptions.Find(userSubscriptionId);
        }


        public List<UserSubscription> GetUserSubscriptionsByUser(string SubscriberId)
        {
            return context.UserSubscriptions.Where(m => m.SubscriberId == SubscriberId).ToList();
        }


        public List<UserSubscription> GetUserSubscriptionsByPublisher(string publisherId)
        {
            return context.UserSubscriptions.Where(m => m.PublishserId == publisherId).ToList();
        }
    }
}
