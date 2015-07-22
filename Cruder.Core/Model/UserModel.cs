using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cruder.Core.Model
{
    public class UserModel
    {
        public int Id { get; set; }

        public string Fullname { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Mail { get; set; }

        public bool IsSystemAdmin { get; set; }

        public UserGroupModel UserGroup { get; set; }
    }
}
