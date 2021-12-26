﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Common
{
    public class GetCatIDPostPagingRequest : PagingRequestBase
    {
        [Required(ErrorMessage = "Vui lòng chọn danh mục cần tìm")]
        public int? CategoryID { get; set; }
    }
}