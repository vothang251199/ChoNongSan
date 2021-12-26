using ChoNongSan.ViewModels.Requests.Common.Accounts;
using ChoNongSan.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChoNongSan.ApiService
{
    public interface IUserApi
    {
        Task<string> Login(LoginRequest request);
    }
}