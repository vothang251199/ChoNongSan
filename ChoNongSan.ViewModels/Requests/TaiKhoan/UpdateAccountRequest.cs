using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Requests.TaiKhoan
{
    public class UpdateAccountRequest
    {
        public int AccountID { get; set; }

        [Display(Name = "Họ và Tên")]
        [Required(ErrorMessage = "Vui lòng nhập Họ và Tên đầy đủ của bạn")]
        public string FullName { get; set; }

        [Display(Name = "Tên tài khoản")]
        public string UserName { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập Số điện thoại")]
        [RegularExpression(@"^((09|03|07|08|05)((\d){8}))$", ErrorMessage = "Số điện thoại không đúng định dạng")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }

        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Vui lòng nhập Địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public IFormFile ThumbnailImage { get; set; }
    }
}