using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Exceptions;
using Microsoft.AspNet.Identity;
using SendGrid;

namespace Core.Utilities
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            return ConfigSendGridAsync(message);
        }

        // Use NuGet to install SendGrid (Basic C# client lib) 
        private static Task ConfigSendGridAsync(IdentityMessage message)
        {
            var myMessage = ConfigureMessage(message);

            var credentials = new NetworkCredential(
                ConfigurationManager.AppSettings["mailAccount"],
                ConfigurationManager.AppSettings["mailPassword"]
            );

            // Create a Web transport for sending email.
            var transportWeb = new Web(credentials);
            // Send the email.
            return SendEmailAsync(transportWeb, myMessage);
        }

        private static async Task SendEmailAsync(ITransport transportWeb, ISendGrid myMessage)
        {
            if (transportWeb != null)
            {
                try
                {
                    await transportWeb.DeliverAsync(myMessage);
                }
                catch (InvalidApiRequestException exception)
                {
                    var details = new StringBuilder();

                    details.Append("ResponseStatusCode: " + exception.ResponseStatusCode + ".   ");
                    for (var i = 0; i < exception.Errors.Count(); i++)
                        details.Append(" -- Error #" + i + " : " + exception.Errors[i]);

                    throw new ApplicationException(details.ToString(), exception);
                }
            }
            else
            {
                Trace.TraceError("Failed to create Web transport.");
                await Task.FromResult(0);
            }
        }

        private static SendGridMessage ConfigureMessage(IdentityMessage message)
        {
            var myMessage = new SendGridMessage();
            myMessage.AddTo(message.Destination);
            myMessage.From = new MailAddress(
                "szymon.pitula@codeteam.pl", "Szymon Pituła");
            myMessage.Subject = message.Subject;
            myMessage.Text = message.Body;
            myMessage.Html = message.Body;
            return myMessage;
        }
    }
}