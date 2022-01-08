using ChoNongSan.Application.Admin.ManagementCTVes;
using ChoNongSan.Application.Common.Accounts;
using ChoNongSan.Data.Models;
using ChoNongSan.Utilities.Extenstions;
using ChoNongSan.ViewModels.Requests.TaiKhoan;
using ChoNongSan.ViewModels.Requests.TaiKhoan.Ctv;
using ChoNongSan.ViewModels.Requests.TaiKhoan.KhachHang;
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
        private readonly IManagementCtvService _mgtCtvService;

        public AccountsController(IAccountService accountService, IManagementCtvService mgtCtvService,
            ChoNongSanContext context)
        {
            _mgtCtvService = mgtCtvService;
            _accountService = accountService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccount()
        {
            return Ok(await _accountService.GetAll());
        }

        [HttpGet("so-dien-thoai/{phoneNumber}")]
        public async Task<IActionResult> GetAccountByPhone([FromRoute] string phoneNumber)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber && x.IsDelete == false);
            if (user == null)
                return BadRequest(new { message = "Tài khoản không tồn tại", status = "FAILED" });
            var result = await _accountService.GetAccountByPhone(phoneNumber);
            return Ok(new { data = result, status = "OK" });
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

            if (user == null) return BadRequest(new { message = "Thông tin đăng nhập không chính xác", status = "FAILED" });
            if (user.IsDelete == true) return BadRequest(new { message = "Tài khoản đã bị khóa", status = "FAILED" });

            var pass = (request.Password + user.KeySecurity.Trim()).ToMD5();
            if (user.Password != pass) return BadRequest(new { message = "Thông tin đăng nhập không chính xác", status = "FAILED" });
            var result = await _accountService.Login(request);

            return Ok(new { data = result, status = "OK" });
        }

        [HttpPost("dang-ky-khach-hang")]
        [AllowAnonymous]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Register([FromForm] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userExist = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.UserName == request.UserName.ToLower());
            var phoneExist = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);
            var emailExist = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.Email == request.Email.ToLower());

            if (userExist != null) return BadRequest(new { message = "Tên tài khoản đã tồn tại", status = "FAILED" });
            if (phoneExist != null) return BadRequest(new { message = "Số điện thoại đã tồn tại", status = "FAILED" });
            if (emailExist != null) return BadRequest(new { message = "Email đã tồn tại.", status = "FAILED" });

            var accountId = await _accountService.Register(request);
            if (accountId == 0) return BadRequest(new { message = "Đăng ký tài khoản thất bại", status = "FAILED" });

            var account = await _accountService.GetAccountById(accountId);
            return Ok(new { message = "Đăng ký tài khoản thành công", status = "OK", data = account });
        }

        [HttpPut("cap-nhat-tai-khoan/{AccountID}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromRoute] int AccountID, [FromForm] UpdateAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.AccountID = AccountID;

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

        [HttpPost("quen-mat-khau")]
        public async Task<IActionResult> ForgotPass([FromBody] ForgetPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.Email == request.Email);
            if (user == null) return BadRequest(new { message = "Email không tồn tại trên hệ thống", status = "FAILED" });
            var result = _accountService.ForgotPassword(request);
            return Ok(new { message = result, status = "OK" });
        }

        [HttpPut("khoi-phuc-mat-khau")]
        public async Task<IActionResult> ResetPass([FromBody] ResetPassRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.Email == request.Email);
            if (user == null) return Ok(new { message = "Email không tồn tại trên hệ thống", status = "FAILED" });
            var result = await _accountService.ResetPassword(request);
            return Ok(new { message = result, status = "OK" });
        }

        [HttpGet("{accountID}")]
        public async Task<IActionResult> GetAccountById(int accountID)
        {
            var cat = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.AccountId == accountID && x.IsDelete == false);
            if (cat == null) return BadRequest(new { message = "Không tìm thấy tài khoản", status = "FAILED" });

            var result = await _accountService.GetAccountById(accountID);
            return Ok(new { data = result, status = "OK" });
        }

        //Amin Management CTV
        [HttpPost("them-ctv")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateCTV([FromForm] CreateCTVRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userExist = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.UserName == request.UserName.ToLower());
            if (userExist != null)
                return BadRequest(new { message = "Tài khoản đã tồn tại", status = "FAILED" });

            var result = await _mgtCtvService.CreateCTV(request);
            if (result == 0)
                return BadRequest(new { message = "Tạo Cộng Tác Viên không thành công", status = "FAILED" });

            return Ok(new { message = "Tạo Cộng Tác Viên thành công", status = "OK" });
        }

        [HttpPut("doi-mat-khau-ctv/{CtvID}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdatePassCTV([FromRoute] int CtvID, [FromForm] UpdatePassCTVRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.AccountID = CtvID;
            var user = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.AccountId == request.AccountID && x.IsDelete == false);
            if (user == null)
                return BadRequest(new { message = "Tài khoản không tồn tại", status = "FAILED" });

            var result = await _mgtCtvService.UpdatePassCTV(request);
            if (result == 0)
                return BadRequest(new { message = "Đổi mật khẩu không thành công", status = "FAILED" });

            return Ok(new { message = "Đổi mật khẩu thành công", status = "OK" });
        }
    }
}