using MedicalJournalWebApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business.Entites;
using System.IO;

namespace MedicalJournalWebApp.Controllers
{
    [Authorize(Roles = "Subscriber")]
    public class SubscribeController : Controller
    {
        // GET: Susbscribe
        public ActionResult Index()
        {
            RestClientBase<Journal> apiCall = new RestClientBase<Journal>("UserSubscriptions/GetUserSubscriptionsByUser");
            return View(apiCall.GetAll());
        }

        public ActionResult Details(int id)
        {
            RestClientBase<Journal> apiCall = new RestClientBase<Journal>("Journals");
            Journal currentJournal = apiCall.GetById(id);

            var fileStream = new FileStream(currentJournal.FileName,
                                     FileMode.Open,
                                     FileAccess.Read
                                   );
            var fsResult = new FileStreamResult(fileStream, "application/pdf");
            Response.AppendHeader("content-disposition", "inline; filename=" + currentJournal.Description + ".pdf");
            return fsResult;

        }

    }
}