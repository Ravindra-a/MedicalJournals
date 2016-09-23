using Business.Entites;
using MedicalJournalWebApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace MedicalJournalWebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            RestClientBase<Journal> apiCall = new RestClientBase<Journal>("Journals");
            return View(apiCall.GetAll());
        }

        
        public ActionResult Subscribe(string publisherId)
        {
            UserSubscription newSubscription = new UserSubscription();
            newSubscription.PublishserId = publisherId;
            newSubscription.SubscriberId = this.User.Identity.GetUserId();
            RestClientBase<UserSubscription> apiCall = new RestClientBase<UserSubscription>("UserSubscriptions");
            apiCall.Add(newSubscription);
            return RedirectToAction("SubscribeConfirmation");
        }

        public ActionResult SubscribeConfirmation()
        {
            return View();
        }
    }
}