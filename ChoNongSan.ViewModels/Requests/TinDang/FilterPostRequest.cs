using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Requests.TinDang
{
	public class FilterPostRequest
	{
		public decimal MinPrice { get; set; }
		public decimal MaxPrice { get; set; }
		public int CategoryId { get; set; }
		public string SortPost { get; set; }
		public string Quality { get; set; }
		public string IsDeliver { get; set; }
	}
}