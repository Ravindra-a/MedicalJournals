using Business.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public interface IUserSubscription
    {
        void Add(UserSubscription userSubscription);
        void Delete(int userSubscriptionId);
        List<UserSubscription> GetUserSubscriptions();
        List<UserSubscription> GetUserSubscriptionsByUser(string subscriberId);
        List<UserSubscription> GetUserSubscriptionsByPublisher(string publisherId);
        UserSubscription GetSubscriptionById(int userSubscriptionId);

    }
}
