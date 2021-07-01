using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models.Account
{
    public class UserRoleModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsSelected { get; set; } = false;
    }
}
