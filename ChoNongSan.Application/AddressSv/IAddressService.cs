using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.Application.AddressSv
{
	public interface IAddressService
	{
		List<AddressVm> GetListDistrict(int code​​Province);

		List<AddressVm> GetListSubDistrict(int codeProvince, int codeDistrict);
	}

	public class AddressService : IAddressService
	{
		private readonly ChoNongSanContext _context;

		public AddressService(ChoNongSanContext context)
		{
			_context = context;
		}

		public List<AddressVm> GetListDistrict(int code​​Province)
		{
			string url = $"https://provinces.open-api.vn/api/?depth=3";

			List<AddressVm> lsDistrict = null;
			using (var webClient = new System.Net.WebClient())
			{
				var json = webClient.DownloadString(url);
				// Now parse with JSON.Net
				var datax = (JArray)JsonConvert.DeserializeObject(json);
				for (var i = 0; i < datax.Count; i++)
				{
					var itemProperties = datax[i].Children<JProperty>();
					//you could do a foreach or a linq here depending on what you need to do exactly with the value
					var myElement = itemProperties.FirstOrDefault(x => x.Name == "code");
					var myElementValue = myElement.Value; ////This is a JValue type
					if (Convert.ToInt32(myElementValue) == codeProvince)
					{
						lsDistrict = JsonConvert.DeserializeObject<List<AddressVm>>(Convert.ToString(datax[i]["districts"]));
						break;
					}
				}
			}

			return lsDistrict;
		}

		public List<AddressVm> GetListSubDistrict(int codeProvince, int codeDistrict)
		{
			string url = $"https://provinces.open-api.vn/api/?depth=3";

			List<AddressVm> lsSubDistrict = null;
			using (var webClient = new System.Net.WebClient())
			{
				var json = webClient.DownloadString(url);
				// Now parse with JSON.Net
				var datax = (JArray)JsonConvert.DeserializeObject(json);
				for (var i = 0; i < datax.Count; i++)
				{
					var codePro = datax[i].Children<JProperty>().FirstOrDefault(x => x.Name == "code").Value;
					if (Convert.ToInt32(codePro) == codeProvince)
					{
						var dtx = (JArray)JsonConvert.DeserializeObject(Convert.ToString(datax[i]["districts"]));
						foreach (var item in dtx)
						{
							var codeDis = Convert.ToInt32(item.Children<JProperty>().FirstOrDefault(p => p.Name == "code").Value);
							if (codeDis == codeDistrict)
							{
								lsSubDistrict = JsonConvert.DeserializeObject<List<AddressVm>>(Convert.ToString(item["wards"]));
								break;
							}
						}
						break;
					}
				}
			}

			return lsSubDistrict;
		}
	}
}