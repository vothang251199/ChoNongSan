using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Requests.TinDang
{
	public class CreatePostRequest
	{
		[Required]
		public string PlatForm { get; set; }

		[Display(Name = "Tiêu đề")]
		[Required(ErrorMessage = "Vui lòng nhập Tiêu đề")]
		public string Title { get; set; }

		[Display(Name = "Mô tả")]
		public string Description { get; set; }

		[Display(Name = "Khối lượng")]
		[Required(ErrorMessage = "Vui lòng nhập Khối lượng")]
		public int? WeightNumber { get; set; }

		[Display(Name = "Giá")]
		[Required(ErrorMessage = "Vui lòng nhập Giá")]
		public decimal? Price { get; set; }

		[Display(Name = "Chất lượng")]
		[Required(ErrorMessage = "Vui lòng chọn chất lượng")]
		public string Quality { get; set; }

		[Display(Name = "Địa chỉ")]
		[Required(ErrorMessage = "Vui lòng nhập Địa chỉ")]
		public string Address { get; set; }

		[Display(Name = "Số điện thoại")]
		[Required(ErrorMessage = "Vui lòng nhập Số điện thoại")]
		[RegularExpression(@"^((09|03|07|08|05)((\d){8}))$", ErrorMessage = "Số điện thoại không đúng định dạng")]
		public string PhoneNumber { get; set; }

		[Display(Name = "Thời hạn bảo quản")]
		[Required(ErrorMessage = "Vui lòng nhập Số ngày")]
		public int? Expiry { get; set; }

		[Display(Name = "Giao hàng")]
		public bool IsDeliver { get; set; }

		public int? AccountID { get; set; }

		[Display(Name = "Danh mục")]
		[Required(ErrorMessage = "Vui lòng chọn Danh mục")]
		public int? CategoryID { get; set; }

		[Display(Name = "Đơn vị")]
		[Required(ErrorMessage = "Vui lòng chọn Đơn vị")]
		public int? WeightId { get; set; }

		[Display(Name = "Tỉnh/ Thành phố")]
		public int Province { get; set; }

		[Display(Name = "Quận /Huyện")]
		public int District { get; set; }

		[Display(Name = "Phường/ Xã")]
		public int SubDistrict { get; set; }

		[Display(Name = "Lat")]
		public string Lat { get; set; }

		[Display(Name = "Lng")]
		public string Lng { get; set; }

		[Display(Name = "Ảnh sản phẩm")]
		[Required(ErrorMessage = "Vui lòng chọn Ảnh")]
		public List<IFormFile> ThumbnailImage { get; set; }
	}
}