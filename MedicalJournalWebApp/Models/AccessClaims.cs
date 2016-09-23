using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalJournalWebApp.Models
{
    public class AccessClaims
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string Cookie { get; set; }
        public string expires_in { get; set; }
        public string client_id { get; set; }
        public string userName { get; set; }

    }
}