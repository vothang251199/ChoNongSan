using ChoNongSan.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Responses
{
    public class PostVmTongQuat
    {
        public int PostID { get; set; }

        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [Display(Name = "Giá")]
        public decimal? Price { get; set; }

        [Display(Name = "Khối lượng")]
        public int? WeightNumber { get; set; }

        [Display(Name = "Loại đơn vị")]
        public string WeightName { get; set; }

        [Display(Name = "Tên người bán")]
        public string NameAccount { get; set; }

        [Display(Name = "Ảnh mặc định")]
        public string ImageDefault { get; set; }

        [Display(Name = "Lượt xem")]
        public int? ViewCount { get; set; }

        [Display(Name = "Vĩ độ")]
        public string Lat { get; set; }

        [Display(Name = "Kinh độ")]
        public string Lng { get; set; }

        [Display(Name = "Trạng thái")]
        public int? StatusPost { get; set; }

        [Display(Name = "Ngày đăng")]
        public DateTime? TimePost { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Display(Name = "Lý do")]
        public string Reason { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "Địa chỉ")]
        public int NumImg { get; set; }
    }
}