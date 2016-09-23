using Business.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public interface IJournal
    {
        void Add(Journal journal);
        void Delete(int journalId);
        List<Journal> GetJournals();
        Journal GetJournalById(int journalId);

        List<Journal> GetJournalsByPublisher(string publisherId);

        List<Journal> GetJournalsByUserSusbscription(List<string> subscribedPublishers);
    }
}
