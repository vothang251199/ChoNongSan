using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Requests.Common.Banners
{
    public class EditBannerRequest
    {
        public int BannerId { get; set; }
        public IFormFile ThumbnailImage { get; set; }
        public string Topic { get; set; }
        public bool? IsDelete { get; set; }
    }
}