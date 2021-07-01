using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Models.Account
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Không để trống")]
        [Display(Name = "Nhập email của bạn")]
        [DataType(DataType.EmailAddress)]
        [StringLength(100, ErrorMessage = "Nhập đúng email")]
        public string Email { set; get; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Quên mật khẩu?")]
        public bool RememberMe { get; set; }
    }
}
