using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Entites;

namespace Repositories.Repositories
{
    public class JournalRepository :IJournal
    {
        MedicalJournalContext context = new MedicalJournalContext();
        public void Add(Journal journal)
        {
            context.Journals.Add(journal);
            context.SaveChanges();
        }

        public void Delete(int journalId)
        {
            Journal j = context.Journals.Find(journalId);
            if (j != null)
            {
                context.Journals.Remove(j);
                context.SaveChanges();
            }
        }

        public List<Journal> GetJournals()
        {
            return context.Journals.ToList();
        }

        public Journal GetJournalById(int journalId)
        {
            return context.Journals.Find(journalId);
        }

        public List<Journal> GetJournalsByPublisher(string publisherId)
        {            
            return context.Journals.Where(m => m.Id == publisherId).ToList();
        }


        public List<Journal> GetJournalsByUserSusbscription(List<string> subscribedPublishers)
        {
            return context.Journals.Where(m => subscribedPublishers.Contains(m.Id)).ToList();
        }
    }
}
