using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Business.Entites.Roles
{
    [DataContract]
    public class UsersInRoleModel
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public List<string> EnrolledUsers { get; set; }
    }
}
