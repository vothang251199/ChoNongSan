using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Responses
{
	public class ReviewVm
	{
		public int ReviewsId { get; set; }
		public int PostId { get; set; }
		public string Avatar { get; set; }
		public string Name { get; set; }
		public string Contents { get; set; }
		public int? NumberOfReviews { get; set; }
		public DateTime? Time { get; set; }
	}
}