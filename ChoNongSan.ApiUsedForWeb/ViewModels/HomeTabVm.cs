﻿using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ApiUsedForWeb.ViewModels
{
    public class HomeTabVm
    {
        public int ActiveTab { get; set; }

        public PageResult<PostVmTongQuat> Data { get; set; }

        public List<CategoryVm> ListCat { get; set; }
        public CategoryVm Cat { get; set; }
    }
}