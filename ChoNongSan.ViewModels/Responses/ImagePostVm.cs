﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Responses
{
    public class ImagePostVm
    {
        public int ImageID { get; set; }
        public string ImagePath { get; set; }
        public bool? IsDefault { get; set; }
        public int PostID { get; set; }
    }
}