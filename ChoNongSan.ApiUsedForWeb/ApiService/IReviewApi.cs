using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.DanhGia;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ApiUsedForWeb.ApiService
{
	public interface IReviewApi
	{
		Task<string> Create(ReviewRequest request);

		Task<string> GetListReview(int postId, GetPagingCommonRequest request);
	}

	public class ReviewApi : IReviewApi
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _config;

		public ReviewApi(IHttpClientFactory httpClientFactory, IConfiguration config)
		{
			_httpClientFactory = httpClientFactory;
			_config = config;
		}

		public async Task<string> Create(ReviewRequest request)
		{
			var json = JsonConvert.SerializeObject(request);
			var httpContnet = new StringContent(json, Encoding.UTF8, "application/json");

			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri(_config["ApiUrl"]);

			var response = await client.PostAsync("/api/danhgia/tao-danh-gia", httpContnet);

			var data = await response.Content.ReadAsStringAsync();
			return data;
		}

		public async Task<string> GetListReview(int postId, GetPagingCommonRequest request)
		{
			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri(_config["ApiUrl"]);
			var response = await client.GetAsync($"/api/danhgia/danh-sach-danh-gia/{postId}?PageIndex={request.PageIndex}&PageSize={request.PageSize}");

			var data = await response.Content.ReadAsStringAsync();
			return data;
		}
	}
}