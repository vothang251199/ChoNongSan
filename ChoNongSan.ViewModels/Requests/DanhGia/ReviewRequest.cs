using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Requests.DanhGia
{
	public class ReviewRequest
	{
		public int PostId { get; set; }
		public int AccountId { get; set; }
		public string Noidung { get; set; }
		public int? Sao { get; set; }
	}
}