using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Requests.DonVi
{
    public class CreateWeightRequest
    {
        [Display(Name = "Tên đơn vị")]
        [Required(ErrorMessage = "Vui lòng nhập Tên đơn vị")]
        public string WeightName { get; set; }
    }
}