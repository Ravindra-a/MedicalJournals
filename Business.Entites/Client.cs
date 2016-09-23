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
    public class Client 
    {
        [Key]
        [DataMember]
        public string ClientId { get; set; }
        [DataMember]
        public string Secret { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public bool Active { get; set; }
        [DataMember]
        public string AllowedOrigin { get; set; }
    }

 }
