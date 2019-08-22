using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
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

        // Use NuGet to install SendGrid (Basic C# client lib) 
        private static async Task ConfigSendGridAsync(IdentityMessage message)
        {
            var credentials = new NetworkCredential(
                ConfigurationManager.AppSettings["mailAccount"],
                ConfigurationManager.AppSettings["mailPassword"]
            );

            // Send the email.
            await SendEmailAsync(credentials, message);
        }

        private static async Task SendEmailAsync(NetworkCredential credentials, IdentityMessage message)
        {
            try
            {
                MailMessage mail = new MailMessage("szymon.pitula@codeteam.pl", message.Destination);
                SmtpClient client = new SmtpClient("smtp-u9hna.vipserv.org");

                mail.Subject = message.Subject;
                mail.Body = message.Body;
                mail.BodyEncoding = Encoding.UTF8;

                client.Port = 587;
                client.UseDefaultCredentials = false;
                client.EnableSsl = false;
                client.Credentials = credentials;
                client.Send(mail);
            }
            catch (Exception e)
            {
                // ignored
            } 
        }
    }
}