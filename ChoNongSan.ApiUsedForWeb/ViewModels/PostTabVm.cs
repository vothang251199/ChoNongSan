using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Responses;
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
		public int byId { get; set; }
		public PageResult<PostVmTongQuat> Data { get; set; }
	}

	public enum Tab
	{
		HienThi,
		DoiDuyet,
		TuChoi,
		DaAn
	}
}