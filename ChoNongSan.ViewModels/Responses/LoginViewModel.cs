using ChoNongSan.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ViewModels.Responses
{
    public class LoginViewModel
    {
        public Account account { get; set; }
        public string token { get; set; }
    }
}