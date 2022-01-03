using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Requests.Common.Accounts;
using ChoNongSan.ViewModels.Responses;
using ChoNongSan.ViewModels.Responses.TaiKhoan;
using ChoNongSan.ViewModels.Responses.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChoNongSan.Application.Common.Accounts
{
    public interface IAccountService
    {
        Task<List<Account>> GetAll();

        Task<AccountVm> GetAccountById(int accountID);

        Task<int> Register(RegisterRequest request);

        Task<LoginViewModel> Login(LoginRequest request);

        Task<int> Update(UpdateAccountRequest request);

        Task<bool> Delete(int AccountID);

        Task<int> ChangePassword(ChangePassRequest request);

        string ForgotPassword(ForgetPasswordRequest request);

        Task<string> ResetPassword(ResetPassRequest request);

        Task<Account> GetAccountByPhone(string phone);
    }
}