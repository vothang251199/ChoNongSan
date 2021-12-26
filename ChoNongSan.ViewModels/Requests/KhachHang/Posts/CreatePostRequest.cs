using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Requests.KhachHang.Posts
{
    public class CreatePostRequest
    {
        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề cho bài viết")]
        public string Title { get; set; }

        [Display(Name = "Tên sản phẩm")]
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        public string ProductName { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Display(Name = "Khối lượng")]
        [Required(ErrorMessage = "Vui lòng nhập khối lượng cho sản phẩm")]
        public int WeightNumber { get; set; }

        [Display(Name = "Giá")]
        [Required(ErrorMessage = "Vui lòng nhập giá cho sản phẩm")]
        public decimal Price { get; set; }

        [Display(Name = "Chất lượng")]
        [Required(ErrorMessage = "Vui lòng chọn chọn chất lượng sản phẩm của bạn")]
        public string Quality { get; set; }

        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Vui lòng nhập Địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập Số điện thoại")]
        [RegularExpression(@"^((09|03|07|08|05)((\d){8}))$", ErrorMessage = "Số điện thoại không đúng định dạng")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Giao hàng")]
        [Required(ErrorMessage = "Vui lòng chọn Có hoặc Không")]
        public bool IsDeliver { get; set; }

        public int AccountID { get; set; }
        public int CategoryID { get; set; }
        public int WeightID { get; set; }

        [Display(Name = "Ảnh sản phẩm")]
        public IFormFile ThumbnailImage { get; set; }
    }
}