using Certification_System.ServicesInterfaces;
using Certification_System.ServicesInterfaces.Models;
using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

namespace Certification_System.Services
{
    public class EmailSender : IEmailSender
    {
        private IEmailConfiguration _emailConfiguration;

        public EmailSender(IEmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }

        public Task SendEmailAsync(EmailMessageDto emailMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailConfiguration.SenderName, _emailConfiguration.SmtpUsername));
            message.To.Add(new MailboxAddress(emailMessage.ReceiverName, emailMessage.ReceiverEmailAddress));
            message.Subject = emailMessage.Subject;

            message.Body = new TextPart("html")
            {
                Text = emailMessage.BodyMessage
            };

//            var builder = new BodyBuilder();

//            // Set the plain-text version of the message text
//            builder.TextBody = @"Hey Alice,

//What are you up to this weekend? Monica is throwing one of her parties on
//Saturday and I was hoping you could make it.

//Will you be my +1?

//-- Joey
//";

//            // In order to reference selfie.jpg from the html text, we'll need to add it
//            // to builder.LinkedResources and then use its Content-Id value in the img src.
//            var image = builder.LinkedResources.Add(@"C:\Users\Joey\Documents\Selfies\selfie.jpg");
//            image.ContentId = MimeUtils.GenerateMessageId();

//            // Set the html version of the message text
//            builder.HtmlBody = string.Format(@"<p>Hey Alice,<br>
//<p>What are you up to this weekend? Monica is throwing one of her parties on
//Saturday and I was hoping you could make it.<br>
//<p>Will you be my +1?<br>
//<p>-- Joey<br>
//<center><img src=""cid:{0}""></center>", image.ContentId);

//            // We may also want to attach a calendar event for Monica's party...
//            builder.Attachments.Add(@"C:\Users\Joey\Documents\party.ics");

//            // Now we just need to set the message body and we're done
//            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, false);

                client.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);

                client.Send(message);
                client.Disconnect(true);
            }

            return Task.CompletedTask;
        }
    }
}

// todo: generator of Body for different purpouses
