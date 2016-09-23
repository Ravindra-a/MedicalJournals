using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Business.Entites.Auth
{
    [DataContract]
    public class RoleReturnModel
    {
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public List<string> PermissionList { get; set; }
    }   
}