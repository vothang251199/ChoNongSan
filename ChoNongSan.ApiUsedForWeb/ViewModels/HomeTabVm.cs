using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.TinDang;
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
		public List<PostVmTongQuat> ListManyViews { get; set; }
		public List<PostVmTongQuat> ListPostNew { get; set; }
		public FilterPostRequest RequestFilterPost { get; set; }
		public List<CategoryVm> ListCat { get; set; }
		public string keyword { get; set; }
	}
}