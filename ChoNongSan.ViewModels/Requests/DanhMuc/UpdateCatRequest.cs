using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Requests.DanhMuc
{
    public class UpdateCatRequest
    {
        [Display(Name = "Mã danh mục")]
        public int CatID { get; set; }

        [Display(Name = "Tên danh mục")]
        [Required(ErrorMessage = "Vui lòng nhập tên dannh mục")]
        public string CatName { get; set; }
    }
}