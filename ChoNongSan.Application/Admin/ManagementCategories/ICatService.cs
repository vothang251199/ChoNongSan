using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.Admin.ManagementCategory;
using ChoNongSan.ViewModels.Responses.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        Task<PageResult<CategoryVm>> GetCatsPaging(GetCatsPagingRequest request);
    }
}