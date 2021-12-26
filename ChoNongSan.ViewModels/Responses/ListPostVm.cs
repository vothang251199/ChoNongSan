using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Responses
{
    public class ListPostVm
    {
        public int PostID { get; set; }

        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; }

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

        [Display(Name = "Danh mục")]
        public string CatName { get; set; }
    }
}