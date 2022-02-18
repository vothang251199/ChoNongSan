using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Responses
{
	public class MeetVm
	{
		public DateTime ThoiGian { get; set; }
		public string TenNguoiTaoLich { get; set; }
		public string PhoneNumber { get; set; }
		public string Title { get; set; }
		public int StatusMeet { get; set; }
		public int MeetId { get; set; }
	}
}