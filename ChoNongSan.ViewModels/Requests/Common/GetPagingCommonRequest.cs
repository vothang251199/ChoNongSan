using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.TinDang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Requests.Common
{
	public class GetPagingCommonRequest : PagingRequestBase
	{
		public string Keyword { get; set; }
		public int? ById { get; set; }
		public int Roles { get; set; }
		public FilterPostRequest RequestFilterPost { get; set; }
	}
}