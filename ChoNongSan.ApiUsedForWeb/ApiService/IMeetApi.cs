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
	}
}