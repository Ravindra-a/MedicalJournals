using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Business.Entites
{
    [DataContract]
    public class UserSubscription
    {
        [Key]
        [DataMember]        
        public int UserSubscriptionId { get; set; }        
        [DataMember]
        public string SubscriberId { get; set; } //foreign key
        [DataMember]
        public string PublishserId { get; set; } //foreign key
        [ForeignKey("PublishserId")]
        public ApplicationUser ApplicationUser { get; set; }
        
    }
}
