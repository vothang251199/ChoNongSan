using ChoNongSan.ViewModels.Requests.Admin;
using ChoNongSan.ViewModels.Requests.Admin.ManagementCTV;
using ChoNongSan.ViewModels.Responses.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.Application.Admin.ManagementCTVes
{
    public interface IManagementCtvService
    {
        Task<List<ListCtvViewModel>> GetListCtv();

        Task<int> CreateCTV(CreateCTVRequest request);

        Task<int> UpdatePassCTV(UpdatePassCTVRequest request);

        Task<int> DeleteCTV(int AccountID);
    }
}