using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Common.Banners;
using ChoNongSan.ViewModels.Requests.Common.Banners;
using ChoNongSan.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.Application.Common.Banners
{
    public interface IBannerService
    {
        Task<int> AddBanner(AddBannerRequest request);

        Task<int> EditBanner(EditBannerRequest request);

        Task<bool> RemoveBanner(int BannerID);

        Task<Banner> GetBannerById(int BannerID);

        Task<List<Banner>> GetAll();

        Task<PageResult<Banner>> GetAllPaging(GetBannerPagingRequest request);
    }
}