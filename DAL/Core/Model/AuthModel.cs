using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Core.Model
{
    public class ForgetPasswordInputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Địa chỉ Email hiện tại")]
        public string Email { get; set; }
    }
    public class ResetPasswordInputModel
    {
        public string UserId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} dài từ {2} đến {1} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nhập mật khẩu mới")]
        public string NewPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} dài từ {2} đến {1} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Mật khẩu không giống nhau")]
        [Display(Name = "Nhập lại mật khẩu mới")]
        public string ConfirmNewPassword { get; set; }

        public string Token { get; set; }
    }
}
