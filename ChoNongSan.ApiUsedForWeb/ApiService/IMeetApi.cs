using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.LichHen;
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
	public interface IMeetApi
	{
		Task<string> Create(CreateMeetRequest request);

		Task<string> CheckMeet(int postId, int accountId);

		Task<string> GetListMeet(int accountId, GetPagingCommonRequest request);

		Task<string> DuyetLich(int meetId, int stt);
	}

	public class MeetApi : IMeetApi
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _config;

		public MeetApi(IHttpClientFactory httpClientFactory, IConfiguration config)
		{
			_config = config;
			_httpClientFactory = httpClientFactory;
		}

		public async Task<string> Create(CreateMeetRequest request)
		{
			var json = JsonConvert.SerializeObject(request);
			var httpContnet = new StringContent(json, Encoding.UTF8, "application/json");

			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri(_config["ApiUrl"]);
			var response = await client.PostAsync("/api/meet/tao-lich-hen", httpContnet);

			var data = await response.Content.ReadAsStringAsync();
			return data;
		}

		public async Task<string> CheckMeet(int postId, int accountId)
		{
			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri(_config["ApiUrl"]);
			var response = await client.GetAsync($"/api/meet/check-meet/{postId}/{accountId}");

			var data = await response.Content.ReadAsStringAsync();
			return data;
		}

		public async Task<string> GetListMeet(int accountId, GetPagingCommonRequest request)
		{
			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri(_config["ApiUrl"]);
			var response = await client.GetAsync($"/api/meet/danh-sach-lich-hen/{accountId}?PageIndex={request.PageIndex}&PageSize={request.PageSize}");

			var data = await response.Content.ReadAsStringAsync();
			return data;
		}

		public async Task<string> DuyetLich(int meetId, int stt)
		{
			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri(_config["ApiUrl"]);
			var response = await client.GetAsync($"/api/meet/duyet-lich-hen/{meetId}/{stt}");

			var data = await response.Content.ReadAsStringAsync();
			return data;
		}
	}
}