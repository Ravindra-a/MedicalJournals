using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Business.Entites.Auth
{
    [DataContract]
    public class UsersInRoleModel
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public List<string> EnrolledUsers { get; set; }
        [DataMember]
        public List<string> RemovedUsers { get; set; }
    }
}