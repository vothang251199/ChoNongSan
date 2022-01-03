using ChoNongSan.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Requests.KhachHang.Posts
{
    public class GetPostByStatusRequest : PagingRequestBase
    {
        public int? Status { get; set; }
    }
}