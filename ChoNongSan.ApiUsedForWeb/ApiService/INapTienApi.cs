using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.LichHen;
using ChoNongSan.ViewModels.Requests.NapTien;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.ApiUsedForWeb.ApiService
{
	public interface INapTienApi
	{
		Task<string> CreateOrUpdate(LichSuNapTienRequest request);

		Task<string> GetListNapTien(int accountId, int role, GetPagingCommonRequest request);
	}

	public class NapTienApi : INapTienApi
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _config;

		public NapTienApi(IHttpClientFactory httpClientFactory, IConfiguration config)
		{
			_config = config;
			_httpClientFactory = httpClientFactory;
		}

		public async Task<string> CreateOrUpdate(LichSuNapTienRequest request)
		{
			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri(_config["ApiUrl"]);

			var requestContent = new MultipartFormDataContent();
			if (request.Anhnaptien != null)
			{
				byte[] data;
				using (var br = new BinaryReader(request.Anhnaptien.OpenReadStream()))
				{
					data = br.ReadBytes((int)request.Anhnaptien.OpenReadStream().Length);
				}

				ByteArrayContent bytes = new ByteArrayContent(data);
				requestContent.Add(bytes, "anhnaptien", request.Anhnaptien.FileName);
			}
			requestContent.Add(new StringContent(request.AccountId.ToString()), "accountId");
			requestContent.Add(new StringContent(request.CTV.ToString()), "cTV");
			requestContent.Add(new StringContent(request.Status.ToString()), "status");
			requestContent.Add(new StringContent(request.HisId.ToString()), "hisId");
			requestContent.Add(new StringContent(request.Role.ToString()), "role");
			requestContent.Add(new StringContent(request.Sotien.ToString()), "sotien");
			requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Cachnap) ? "" : request.Cachnap), "cachnap");

			var response = await client.PostAsync("/api/nap-tien/tao-or-cap-nhat", requestContent);

			var data1 = await response.Content.ReadAsStringAsync();
			return data1;
		}

		public async Task<string> GetListNapTien(int accountId, int role, GetPagingCommonRequest request)
		{
			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri(_config["ApiUrl"]);
			var response = await client.GetAsync($"/api/nap-tien/danh-sach-nap-tien/{accountId}/{role}?PageIndex={request.PageIndex}&PageSize={request.PageSize}");

			var data = await response.Content.ReadAsStringAsync();
			return data;
		}
	}
}