using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ApiUsedForWeb.ViewModels
{
    public class PostTabVm
    {
        public Tab ActiveTab { get; set; }
        public int? pageIndex { get; set; }
        public int? pageSize { get; set; }
        public int acountId { get; set; }
    }

    public enum Tab
    {
        HienThi,
        DoiDuyet,
        TuChoi,
        DaAn
    }
}