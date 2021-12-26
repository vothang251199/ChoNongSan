using System.ComponentModel.DataAnnotations;

namespace ChoNongSan.ViewModels.Requests.Admin.ManagementCTV
{
    public class CreateCTVRequest
    {
        [Display(Name = "Tên tài khoản")]
        [Required(ErrorMessage = "Vui lòng nhập Tên tài khoản")]
        public string UserName { get; set; }

        [Display(Name = "Mật khẩu")]
        [MinLength(5, ErrorMessage = "Độ dài mật khẩu tối thiểu 5 ký tự")]
        [Required(ErrorMessage = "Vui lòng nhập Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Xác nhận mật khẩu")]
        [MinLength(5, ErrorMessage = "Độ dài mật khẩu tối thiểu 5 ký tự")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không giống nhau")]
        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; }
    }
}