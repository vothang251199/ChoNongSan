using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Common
{
    public class GetSearchPostPagingRequest : PagingRequestBase
    {
        public string KeyWord { get; set; }
        public int? CategoryID { get; set; }
    }
}