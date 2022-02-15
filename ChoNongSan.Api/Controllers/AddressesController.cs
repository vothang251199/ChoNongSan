using ChoNongSan.Application.AddressSv;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChoNongSan.Api.Controllers
{
	[Route("api/address")]
	[ApiController]
	public class AddressesController : ControllerBase
	{
		private readonly IAddressService _addressService;

		public AddressesController(IAddressService addressService)
		{
			_addressService = addressService;
		}

		[HttpGet("lay-tat-ca-huyen/{code}")]
		public IActionResult GetListDistrcit([FromRoute] int code)
		{
			return Ok(_addressService.GetListDistrict(code));
		}

		[HttpGet("lay-tat-ca-xa/{codeProvince}/{codeDistrict}")]
		public IActionResult GetListSubDistrict([FromRoute] int codeProvince, int codeDistrict)
		{
			return Ok(_addressService.GetListSubDistrict(codeProvince, codeDistrict));
		}
	}
}