using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Auth.Models;
using Exceptions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using SendGrid;

namespace Auth
{
    // Configure the application user manager used in this application.
    // r is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store, IIdentityMessageService emailService,
            IdentityFactoryOptions<ApplicationUserManager> options)
            : base(store)
        {
            EmailService = emailService;

            UserValidator = new UserValidator<ApplicationUser>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true
            };
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
                UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"))
                    {
                        TokenLifespan = TimeSpan.FromHours(3)
                    };
        }

        public static IDataProtectionProvider DataProtectionProvider { get; private set; }
    }

    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            await configSendGridasync(message);
        }

        // Use NuGet to install SendGrid (Basic C# client lib) 
        private async Task configSendGridasync(IdentityMessage message)
        {
            var myMessage = new SendGridMessage();
            myMessage.AddTo(message.Destination);
            myMessage.From = new MailAddress(
                "szymon.pitula@codeteam.pl", "Szymon Pituła");
            myMessage.Subject = message.Subject;
            myMessage.Text = message.Body;
            myMessage.Html = message.Body;

            var credentials = new NetworkCredential(
                ConfigurationManager.AppSettings["mailAccount"],
                ConfigurationManager.AppSettings["mailPassword"]
            );

            // Create a Web transport for sending email.
            //var transportWeb = new Web("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", credentials, TimeSpan.FromSeconds(10));
            // var transportWeb = new Web("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");
            //var transportWeb = new Web("D1fQA_8fRrWxPSwvv3FVyQ");
            var transportWeb = new Web(credentials);
            // Send the email.
            if (transportWeb != null)
            {
                try
                {
                    await transportWeb.DeliverAsync(myMessage);
                }
                catch (InvalidApiRequestException ex)
                {
                    var detalle = new StringBuilder();

                    detalle.Append("ResponseStatusCode: " + ex.ResponseStatusCode + ".   ");
                    for (var i = 0; i < ex.Errors.Count(); i++)
                        detalle.Append(" -- Error #" + i + " : " + ex.Errors[i]);

                    throw new ApplicationException(detalle.ToString(), ex);
                }
            }
            else
            {
                Trace.TraceError("Failed to create Web transport.");
                await Task.FromResult(0);
            }
        }
    }
}