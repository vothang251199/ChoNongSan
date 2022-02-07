using ChoNongSan.ApiUsedForWeb.ApiService;
using ChoNongSan.ApiUsedForWeb.ViewModels;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.TinDang;
using ChoNongSan.ViewModels.Responses;
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

		public async Task<IViewComponentResult> InvokeAsync(PageResult<PostVmTongQuat> model)
		{
			return await Task.FromResult((IViewComponentResult)View("Default", model));
		}
	}
}