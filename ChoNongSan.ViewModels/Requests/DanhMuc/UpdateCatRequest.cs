using Microsoft.AspNetCore.Http;
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
        public string CatName { get; set; }

        [Display(Name = "Ảnh")]
        [Required(ErrorMessage = "Vui lòng ảnh")]
        public IFormFile Image { get; set; }
    }
}