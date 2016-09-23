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
    public class JournalsController : ApiController
    {
        IJournal journalRepo = new JournalRepository();

        // GET: api/Journals
        [AllowAnonymous]
        public List<Journal> GetJournals()
        {
            return journalRepo.GetJournals();
        }

        // GET: api/Journals/5
        [ResponseType(typeof(Journal))]
        public IHttpActionResult GetJournal(int id)
        {
            Journal journal = journalRepo.GetJournalById(id);
            if (journal == null)
            {
                return NotFound();
            }

            return Ok(journal);
        }

        [Route("api/Journals/GetJournalsByPublisher")]
        public List<Journal> GetJournalsByPublisher()
        {
            string publisherId = this.User.Identity.GetUserId();
            return journalRepo.GetJournalsByPublisher(publisherId);
        }
        

        // POST: api/Journals
        [ResponseType(typeof(Journal))]
        [HttpPost]
        public IHttpActionResult PostJournal(Journal journal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            journalRepo.Add(journal);

            return CreatedAtRoute("DefaultApi", new { id = journal.JournalId }, journal);
        }

        // DELETE: api/Journals/5
        [ResponseType(typeof(Journal))]
        public IHttpActionResult DeleteJournal(int id)
        {
            journalRepo.Delete(id);
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