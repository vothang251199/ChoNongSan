using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Responses
{
	public class NapTienVm
	{
		public decimal Sotien { get; set; }
		public string Cachnap { get; set; }
		public string anhnaptien { get; set; }
		public string TenNguoiNap { get; set; }
		public DateTime Time { get; set; }
		public int Status { get; set; }
		public string CTV { get; set; }
		public int HisId { get; set; }
	}
}