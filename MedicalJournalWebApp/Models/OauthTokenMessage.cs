using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalJournalWebApp.Models
{
    /// <summary>
    /// matching with context attributes sent from auth service
    /// </summary>
    public class OauthTokenMessage
    {
        public string error { get; set; }

        public string error_description { get; set; }
    }

    public class APIMessage
    {
        public string Message { get; set; }

        //public string ModelState { get; set; }
    }
}