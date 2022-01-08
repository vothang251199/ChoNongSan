﻿using ChoNongSan.ApiUsedForWeb.ApiService;
using ChoNongSan.ApiUsedForWeb.ViewModels;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.TinDang;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ChoNongSan.Controllers.Components
{
    public class HienThiViewComponent : ViewComponent
    {
        private readonly IPostApi _postApi;
        private readonly IConfiguration _config;

        public HienThiViewComponent(IPostApi postApi, IConfiguration config)
        {
            _config = config;
            _postApi = postApi;
        }

        public async Task<IViewComponentResult> InvokeAsync(PostTabVm model)
        {
            var request = new GetPagingCommonRequest()
            {
                ById = 2,
                PageIndex = (int)model.pageIndex,
                PageSize = (int)model.pageSize,
            };
            var data = await _postApi.GetAllByStatusPaging(request);
            ViewBag.ApiUrl = _config["ApiUrl"];
            return await Task.FromResult((IViewComponentResult)View("Default", data));
        }
    }
}