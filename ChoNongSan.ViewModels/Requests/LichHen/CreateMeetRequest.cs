using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Requests.LichHen
{
	public class CreateMeetRequest
	{
		public int PostId { get; set; }
		public string Date { get; set; }
		public string Time { get; set; }
		public string Phone { get; set; }
		public int NguoiTaoLich { get; set; }
	}
}