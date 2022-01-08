using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Responses
{
    public class WeightVm
    {
        [Display(Name = "Mã đơn vị")]
        public int WeightId { get; set; }

        [Display(Name = "Tên đơn vị")]
        public string WeightName { get; set; }
    }
}