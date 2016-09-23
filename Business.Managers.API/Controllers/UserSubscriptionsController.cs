using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Business.Entites;
using Repositories;
using Repositories.Repositories;
using Microsoft.AspNet.Identity;

namespace Business.Managers.API.Controllers
{
    [Authorize]
    public class UserSubscriptionsController : ApiController
    {
        IUserSubscription userSubscriptionsRepo = new UserSubscriptionRepository();
        IJournal journalRepo = new JournalRepository();

        // GET: api/UserSubscriptions
        public List<UserSubscription> GetUserSubscriptions()
        {
            return userSubscriptionsRepo.GetUserSubscriptions();
        }

        // GET: api/UserSubscriptions/5
        [ResponseType(typeof(UserSubscription))]
        public IHttpActionResult GetUserSubscription(int id)
        {
            UserSubscription userSubscription = userSubscriptionsRepo.GetSubscriptionById(id);
            if (userSubscription == null)
            {
                return NotFound();
            }

            return Ok(userSubscription);
        }

        [Route("api/UserSubscriptions/GetUserSubscriptionsByUser")]
        public List<Journal> GetUserSubscriptionsByUser()
        {
            string subscriberId = this.User.Identity.GetUserId();
            List<UserSubscription> subscribedPublishers =  userSubscriptionsRepo.GetUserSubscriptionsByUser(subscriberId);
            List<string> publisherIds = new List<string>();
            foreach (UserSubscription subscription in subscribedPublishers)
            {
                publisherIds.Add(subscription.PublishserId);
            }
            return (journalRepo.GetJournalsByUserSusbscription(publisherIds));
        }

        [Route("api/UserSubscriptions/GetUserSubscriptionsByPublisher")]
        public List<UserSubscription> GetUserSubscriptionsByPublisher()
        {
            string publisherId = this.User.Identity.GetUserId();
            return userSubscriptionsRepo.GetUserSubscriptionsByPublisher(publisherId);
        }

        
        // POST: api/UserSubscriptions
        [ResponseType(typeof(UserSubscription))]
        public IHttpActionResult PostUserSubscription(UserSubscription userSubscription)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            userSubscriptionsRepo.Add(userSubscription);

            return CreatedAtRoute("DefaultApi", new { id = userSubscription.UserSubscriptionId }, userSubscription);
        }

        // DELETE: api/UserSubscriptions/5
        [ResponseType(typeof(UserSubscription))]
        public IHttpActionResult DeleteUserSubscription(int id)
        {
            userSubscriptionsRepo.Delete(id);

            return Ok(id);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }
    }
}