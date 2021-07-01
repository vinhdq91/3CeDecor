using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models.Account
{
    public class RoleModel
    {
        [Required]
        public string RoleName { get; set; }
        public string Description { get; set; }
    }
}
