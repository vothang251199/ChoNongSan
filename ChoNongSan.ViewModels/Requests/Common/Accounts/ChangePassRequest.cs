using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Requests.Common.Accounts
{
    public class ChangePassRequest
    {
        public int AccountID { get; set; }

        [Display(Name = "Mật khẩu cũ")]
        [MinLength(5, ErrorMessage = "Độ dài mật khẩu tối thiểu 5 ký tự")]
        [Required(ErrorMessage = "Vui lòng nhập Mật khẩu cũ")]
        public string OldPass { get; set; }

        [Display(Name = "Mật khẩu mới")]
        [MinLength(5, ErrorMessage = "Độ dài mật khẩu tối thiểu 5 ký tự")]
        [Required(ErrorMessage = "Vui lòng nhập Mật khẩu mới")]
        public string NewPass { get; set; }

        [Display(Name = "Xác nhận mật khẩu mới")]
        [MinLength(5, ErrorMessage = "Độ dài mật khẩu tối thiểu 5 ký tự")]
        [Compare("NewPass", ErrorMessage = "Xác nhận mật khẩu mới không giống nhau")]
        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới")]
        public string ConfirmNewPass { get; set; }
    }
}