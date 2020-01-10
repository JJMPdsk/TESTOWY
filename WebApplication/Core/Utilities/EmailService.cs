using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Core.Utilities
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            await ConfigSendGridAsync(message);
        }

        private static async Task ConfigSendGridAsync(IdentityMessage message)
        {
            var credentials = new NetworkCredential(
                ConfigurationManager.AppSettings["mailAccount"],
                ConfigurationManager.AppSettings["mailPassword"]
            );

            // Wyślij email
            await SendEmailAsync(credentials, message);
        }

        private static async Task SendEmailAsync(NetworkCredential credentials, IdentityMessage message)
        {
            try
            {
                var mail = new MailMessage
                {
                    From = new MailAddress(ConfigurationManager.AppSettings["mailAccount"], Constants.AppName),
                    Body = message.Body,
                    BodyEncoding = Encoding.UTF8,
                    Subject = message.Subject,
                    To = { message.Destination },
                    IsBodyHtml = true
                };

                var client = new SmtpClient("smtp-u9hna.vipserv.org")
                {
                    Port = 587, UseDefaultCredentials = false,
                    EnableSsl = false,
                    Credentials = credentials
                };

                await client.SendMailAsync(mail);
            }
            catch (Exception e)
            {
                // ignored
            }
        }
    }
}