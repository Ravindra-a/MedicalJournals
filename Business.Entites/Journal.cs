using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Business.Entites
{
    [DataContract]
    public class Journal
    {
        [Key]
        [DataMember]
        public int JournalId { get; set; }
        [DataMember]
        public string JournalName { get; set; }
        [DataMember]
        public string KeyWords { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Id { get; set; } //foreign key to user table i.e. ID of publisher
        [DataMember]
        public string FileName { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
