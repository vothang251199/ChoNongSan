using ChoNongSan.ViewModels.Requests.CTV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.Application.CTV
{
    public interface ICtvService
    {
        Task<int> AcceptPost(AcceptPostRequest request);
    }
}