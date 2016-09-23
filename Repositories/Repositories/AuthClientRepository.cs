using Business.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class AuthClientRepository : IAuthClientRepository
    {
        MedicalJournalContext context = new MedicalJournalContext();
        public Client FindClient(string clientId)
        {
            return context.Clients.Find(clientId);
        }
    }
}
