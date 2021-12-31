using ChoNongSan.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Requests.Admin.ManagementCategory
{
    public class GetCatsPagingRequest : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}