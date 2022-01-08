using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Requests.TaiKhoan.KhachHang
{
    public class RegisterRequest
    {
        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Vui lòng nhập Họ và tên")]
        [MaxLength(20, ErrorMessage = "Nhập tối đa 20 kí tự")]
        public string FullName { get; set; }

        [Display(Name = "Tên tài khoản")]
        [Required(ErrorMessage = "Vui lòng nhập Tên tài khoản")]
        [MinLength(5, ErrorMessage = "Nhập tối thiểu 5 kí tự")]
        [MaxLength(20, ErrorMessage = "Nhập tối đa 20 kí tự")]
        [RegularExpression(@"^([a-zA-Z0-9]+)$", ErrorMessage = "Chỉ được nhập số và chữ không dấu")]
        public string UserName { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập Số điện thoại")]
        [RegularExpression(@"^((09|03|07|08|05)((\d){8}))$", ErrorMessage = "Số điện thoại không đúng định dạng")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }

        [Display(Name = "Mật khẩu")]
        [MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
        [Required(ErrorMessage = "Vui lòng nhập Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Xác nhận mật khẩu")]
        [MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không giống nhau")]
        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; }
    }
}