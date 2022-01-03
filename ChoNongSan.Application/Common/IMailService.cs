using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using System;

namespace ChoNongSan.Application.Common
{
    public interface IMailService
    {
        bool SendEmai(string toEmail, string title, string content);
    }

    public class MailService : IMailService
    {
        private IConfiguration _config;

        public MailService(IConfiguration config)
        {
            _config = config;
        }

        public bool SendEmai(string toEmail, string title, string content)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com");
                    client.Authenticate("chonongsan.vvt.vn@gmail.com", "Ge06vt251199@");

                    var bodyBuilder = new BodyBuilder
                    {
                        HtmlBody = content,
                    };

                    var message = new MimeMessage
                    {
                        Body = bodyBuilder.ToMessageBody(),
                    };

                    message.From.Add(new MailboxAddress("ChoNongSan", "chonongsan.vvt.vn@gmail.com"));
                    message.To.Add(new MailboxAddress("Me", toEmail));
                    message.Subject = title;
                    client.Send(message);
                    client.Disconnect(true);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}