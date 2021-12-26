using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Requests.Admin.ManagementCategory
{
    public class CreateCatRequest
    {
        [Display(Name = "Tên danh mục")]
        [Required(ErrorMessage = "Vui lòng nhập tên dannh mục")]
        public string CatName { get; set; }
    }
}