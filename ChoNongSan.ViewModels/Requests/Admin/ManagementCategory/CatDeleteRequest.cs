using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Requests.Admin.ManagementCategory
{
    public class CatDeleteRequest
    {
        public int CatId { get; set; }
        public string CatName { get; set; }
    }
}