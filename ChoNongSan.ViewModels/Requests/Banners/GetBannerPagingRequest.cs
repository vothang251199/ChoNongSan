using ChoNongSan.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Requests.Banners
{
    public class GetBannerPagingRequest : PagingRequestBase
    {
        public bool? IsDelete { get; set; }
    }
}