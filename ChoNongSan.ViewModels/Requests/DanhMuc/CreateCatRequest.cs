using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Requests.DanhMuc
{
    public class CreateCatRequest
    {
        [Display(Name = "Tên danh mục")]
        [Required(ErrorMessage = "Vui lòng nhập tên dannh mục")]
        public string CatName { get; set; }

        [Display(Name = "Ảnh danh mục")]
        [Required(ErrorMessage = "Vui lòng chọn ảnh")]
        public IFormFile Image { get; set; }
    }
}