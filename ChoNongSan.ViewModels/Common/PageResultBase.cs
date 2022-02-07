using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Common
{
    public class PageResultBase
    {
        public decimal MaxPrice { get; set; }
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int PageCount
        {
            get
            {
                var pageCount = (double)TotalRecords / (PageSize == 0 ? TotalRecords : PageSize);
                return (int)Math.Ceiling(pageCount);
            }
        }
    }
}