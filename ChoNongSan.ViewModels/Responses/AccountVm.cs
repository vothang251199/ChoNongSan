using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Responses
{
    public class AccountVm
    {
        [Display(Name = "Mã tài khoản")]
        public int AccountId { get; set; }

        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Tài khoản")]
        public string UserName { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "Họ tên")]
        public string FullName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string Avatar { get; set; }

        [Display(Name = "Số bài viết")]
        public int? NumberOfPost { get; set; }

        [Display(Name = "Số tiền")]
        public decimal? Money { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime? CreateDate { get; set; }
    }
}