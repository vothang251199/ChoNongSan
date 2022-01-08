using ChoNongSan.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Requests.DanhMuc
{
    public class GetSearchPostPagingRequest : PagingRequestBase
    {
        public string KeyWord { get; set; }
        public int? CategoryID { get; set; }
    }
}