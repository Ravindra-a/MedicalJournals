using Business.Entites;
using MedicalJournalWebApp.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace MedicalJournalWebApp.Controllers
{
    [Authorize(Roles = "Publisher")]
    public class PublishController : Controller
    {
        // GET: Publish
        public ActionResult Index()
        {
            RestClientBase<Journal> apiCall = new RestClientBase<Journal>("Journals/GetJournalsByPublisher");
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

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(Journal model)
        {
            try
            {
                int MaxContentLength = 1024 * 1024 * 3; //3 MB
                string[] AllowedFileExtensions = new string[] { ".pdf"};
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(fileContent.FileName);

                        //validate allowed files
                        if (!AllowedFileExtensions.Contains(fileContent.FileName.Substring(fileContent.FileName.LastIndexOf('.'))))
                        {
                            Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            ModelState.AddModelError("", "Please select file of type: " + string.Join(", ", AllowedFileExtensions));
                            return View(model);
                        }

                        else if (fileContent.ContentLength > MaxContentLength)
                        {
                            Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            ModelState.AddModelError("","Your file is too large, maximum allowed size is: " + MaxContentLength + " MB");
                            return View(model);
                        }
                        else
                        {
                            var path = Path.Combine(ConfigurationManager.AppSettings["PathToUploadFiles"], fileName);
                            fileContent.SaveAs(path);

                            model.FileName = path;
                            model.Id = this.User.Identity.GetUserId();

                            RestClientBase<Journal> apiCall = new RestClientBase<Journal>("Journals");
                            apiCall.Add(model);
                            return RedirectToAction("Index");
                        }
                    }
                }
                ModelState.AddModelError("", "No files to Upload");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }
        
    }
}