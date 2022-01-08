using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Requests.Banners
{
    public class AddBannerRequest
    {
        [Display(Name = "Ảnh Banner")]
        public IFormFile ThumbnailImage { get; set; }

        public string Topic { get; set; }
    }
}