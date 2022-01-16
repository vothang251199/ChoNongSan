using ChoNongSan.ApiUsedForWeb.ApiService;
using ChoNongSan.ApiUsedForWeb.ViewModels;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.TinDang;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ChoNongSan.Controllers.Components
{
    public class DoiDuyetViewComponent : ViewComponent
    {
        private readonly IPostApi _postApi;
        private readonly IConfiguration _config;

        public DoiDuyetViewComponent(IPostApi postApi, IConfiguration config)
        {
            _config = config;
            _postApi = postApi;
        }

        public async Task<IViewComponentResult> InvokeAsync(PostTabVm model)
        {
            var request = new GetPagingCommonRequest()
            {
                ById = 0,
                PageIndex = (int)model.pageIndex,
                PageSize = (int)model.pageSize,
            };

            var data = await _postApi.GetAllByStatusPaging(model.acountId, request);
            foreach (var i in data.Items)
            {
                i.ImageDefault = _config["ApiUrl"] + i.ImageDefault;
            }
            return View(data);
        }
    }
}