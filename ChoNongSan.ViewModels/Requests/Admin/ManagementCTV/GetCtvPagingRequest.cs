﻿using ChoNongSan.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Requests.Admin.ManagementCTV
{
    public class GetCtvPagingRequest : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}