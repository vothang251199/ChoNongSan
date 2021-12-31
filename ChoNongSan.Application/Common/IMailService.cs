using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.Application.Common
{
    public interface IMailService
    {
        Task SendEmai(string toEmail, string title, string content);
    }

    public class MailService : IMailService
    {
        private IConfiguration _config;

        public MailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmai(string toEmail, string title, string content)
        {
            var apiKey = _config["SendGridAPIKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("chonongsan.vvt.vn@gmail.com", "Khôi phục mật khẩu");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, title, content, content);
            var response = await client.SendEmailAsync(msg);
        }
    }
}