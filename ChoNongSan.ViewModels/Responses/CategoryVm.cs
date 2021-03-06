using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Responses
{
    public class CategoryVm
    {
        [Display(Name = "Mã danh mục")]
        public int CategoryID { get; set; }

        [Display(Name = "Tên danh mục")]
        public string CateName { get; set; }

        [Display(Name = "Ảnh")]
        public string Image { get; set; }
    }
}