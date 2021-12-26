﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.Application.KhachHang.PostImages
{
    public class PostImageUpdateRequest
    {
        public int ImageID { get; set; }
        public IFormFile ImagePath { get; set; }
    }
}