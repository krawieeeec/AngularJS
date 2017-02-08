using System.Collections.Generic;

namespace TO.User
{
    public class AddRoleToUserTO
    {
        public string UserName { get; set; }
        public List<string> Roles { get; set; }
    }
}
