using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.DanhMuc;
using ChoNongSan.ViewModels.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChoNongSan.Application.Admin.ManagementCategories
{
    public interface ICatService
    {
        Task<int> CreateCat(CreateCatRequest request);

        Task<int> UpdateCat(UpdateCatRequest request);

        Task<bool> DeleteCat(int CatID);

        Task<List<CategoryVm>> GetListCat();

        Task<CategoryVm> GetCatById(int CatID);

        Task<PageResult<CategoryVm>> GetCatsPaging(GetPagingCommonRequest request);
    }
}