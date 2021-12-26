using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Requests.CTV
{
    public class AcceptPostRequest
    {
        public int PostID { get; set; }
        public int StatustPost { get; set; }
        public string Reason { get; set; }
    }
}