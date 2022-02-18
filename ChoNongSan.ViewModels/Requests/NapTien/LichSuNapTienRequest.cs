using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Requests.NapTien
{
	public class LichSuNapTienRequest
	{
		public decimal Sotien { get; set; }
		public string Cachnap { get; set; }
		public IFormFile Anhnaptien { get; set; }
		public int AccountId { get; set; }
		public int Status { get; set; }
		public int Role { get; set; }
		public int CTV { get; set; }
		public int HisId { get; set; }
	}
}