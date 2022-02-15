using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Responses
{
	public class MoneyVm
	{
		public DateTime Time { get; set; }
		public decimal Money { get; set; }
		public string WhoAdd { get; set; }
	}
}