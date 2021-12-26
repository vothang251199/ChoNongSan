﻿using ChoNongSan.Application.Common.Accounts;
using ChoNongSan.Data.Models;
using ChoNongSan.Utilities.Extenstions;
using ChoNongSan.ViewModels.Requests.Common.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ChoNongSan.Api.Controllers
{
    [Route("api/tai-khoan")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ChoNongSanContext _context;

        public AccountsController(IAccountService accountService,
            ChoNongSanContext context)
        {
            _accountService = accountService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccount()
        {
            return Ok(await _accountService.GetAll());
        }

        [HttpPost("dang-nhap")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.UserName.Contains(request.LoginName.ToLower())
                || x.PhoneNumber.Contains(request.LoginName) || x.Email.Contains(request.LoginName.ToLower()));

            if (user == null) return Ok(new { message = "Thông tin đăng nhập không chính xác", status = "FAILED" });
            if (user.IsDelete == true) return BadRequest(new { message = "Tài khoản đã bị khóa", status = "FAILED" });

            var pass = (request.Password + user.KeySecurity.Trim()).ToMD5();
            if (user.Password != pass) return Ok(new { message = "Thông tin đăng nhập không chính xác", status = "FAILED" });
            var result = await _accountService.Login(request);

            return Ok(new { data = result, status = "OK" });
        }

        [HttpPut("cap-nhat-tai-khoan")]
        public async Task<IActionResult> Update([FromForm] UpdateAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Accounts.FindAsync(request.AccountID);

            if (user == null) return BadRequest(new { message = "Tài khoản không tồn tại", status = "FAILED" });

            var emailExist = await _context.Accounts.AsNoTracking().Where(x => x.Email == request.Email.ToLower()).CountAsync();
            var phoneExist = await _context.Accounts.AsNoTracking().Where(x => x.PhoneNumber == request.PhoneNumber).CountAsync();

            if (user.Email != request.Email.ToLower() || user.PhoneNumber != request.PhoneNumber)
            {
                if (phoneExist > 0)
                    return BadRequest(new { message = "Số điện thoại đã tồn tại", status = "FAILED" });
                if (emailExist > 0)
                    return BadRequest(new { message = "Email đã tồn tại.", status = "FAILED" });
            }

            var result = (await _accountService.Update(request));
            if (result == 1)
                return Ok(new { message = "Cập nhật tài khoản thành công", status = "OK" });
            else
                return BadRequest(new { message = "Cập nhật tài khoản thất bại", status = "FAILED" });
        }

        [HttpDelete("xoa-tai-khoan/{AccountID}")]
        public async Task<IActionResult> Delete(int AccountID)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Accounts.FindAsync(AccountID);
            if (user == null) return BadRequest("Tài khoản không tồn tại");

            var result = await _accountService.Delete(AccountID);
            if (!result) return BadRequest("Xóa tài khoản không thành công");

            return Ok("Xóa tài khoản thành công");
        }

        [HttpPut("doi-mat-khau")]
        public async Task<IActionResult> ChangePass([FromForm] ChangePassRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Accounts.FindAsync(request.AccountID);
            if (user == null) return BadRequest(new { message = "Tài khoản không tồn tại", status = "FAILED" });

            var oldPass = (request.OldPass + user.KeySecurity.Trim()).ToMD5();
            if (oldPass != user.Password) return BadRequest(new { message = "Mật khẩu cũ không đúng", status = "FAILED" });

            var result = await _accountService.ChangePassword(request);
            if (result == 0) return BadRequest(new { message = "Đổi mật khẩu không thành công", status = "FAILED" });

            return Ok(new { message = "Đổi mật khẩu thành công", status = "OK" });
        }
    }
}