using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.TaiKhoan.Ctv;
using ChoNongSan.ViewModels.Responses;
using System.Threading.Tasks;

namespace ChoNongSan.Application.Admin.ManagementCTVes
{
    public interface IManagementCtvService
    {
        Task<PageResult<CtvVm>> GetCtvPaging(GetPagingCommonRequest request);

        Task<int> CreateCTV(CreateCTVRequest request);

        Task<int> UpdatePassCTV(UpdatePassCTVRequest request);

        Task<int> DeleteCTV(int AccountID);
    }
}