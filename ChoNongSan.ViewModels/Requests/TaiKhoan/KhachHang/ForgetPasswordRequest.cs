using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Requests.TaiKhoan.KhachHang
{
    public class ForgetPasswordRequest
    {
        [Display(Name = "Email đăng ký")]
        [Required(ErrorMessage = "Vui lòng nhập Email đã đăng ký")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }
    }
}