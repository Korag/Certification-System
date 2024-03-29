﻿using Certification_System.ServicesInterfaces;
using Certification_System.ServicesInterfaces.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using MimeKit;
using MimeKit.Utils;
using System.IO;
using System.Threading.Tasks;

namespace Certification_System.Services
{
    public class EmailSender : IEmailSender
    {
        private IEmailConfiguration _emailConfiguration;
        private IHostingEnvironment _environment;

        public EmailSender(IEmailConfiguration emailConfiguration, IHostingEnvironment environment)
        {
            _emailConfiguration = emailConfiguration;
            _environment = environment;
        }

        public Task SendEmailAsync(EmailMessageDto emailMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailConfiguration.SenderName, _emailConfiguration.SmtpUsername));
            message.To.Add(new MailboxAddress(emailMessage.ReceiverName, emailMessage.ReceiverEmailAddress));
            message.Subject = emailMessage.Subject;

            var builder = new BodyBuilder();

            builder.TextBody = $@"
++++++++++++++++++++
Certification-System
++++++++++++++++++++

{emailMessage.Header}
*******************************************

{emailMessage.BodyMessage}

*******************************************
{emailMessage.LinkText}: {emailMessage.Link}
______________________
ZIAD Bielsko-Biała SA
al. Armii Krajowej 220
43-316 Bielsko-Biała
";
            var logo = builder.LinkedResources.Add(Path.Combine(_environment.WebRootPath, @"Image\logo_ziad_medium.jpg"));
            logo.ContentId = MimeUtils.GenerateMessageId();
     
            string html = File.ReadAllText(Path.Combine(_environment.WebRootPath, @"resources\emailTemplate\index.htm"));

            builder.HtmlBody = html
                                  .Replace("{ContentId}", logo.ContentId)
                                  .Replace("{Header}", emailMessage.Header)
                                  .Replace("{BodyMessage}", emailMessage.BodyMessage);

            if (!string.IsNullOrWhiteSpace(emailMessage.Link))
            {
                builder.HtmlBody = builder.HtmlBody
                                                 .Replace("display: none", "display: inline-block")
                                                 .Replace("{LinkButtonValue}", emailMessage.Link)
                                                 .Replace("{LinkButtonText}", emailMessage.LinkText);
            };
          
            if (!string.IsNullOrWhiteSpace(emailMessage.ObjectClassifier) && !string.IsNullOrWhiteSpace(emailMessage.Indexer))
            {
                builder.HtmlBody = builder.HtmlBody
                                                 .Replace("Margin-top: 0;Margin-bottom: 0;display:none;", "Margin-top: 0;Margin-bottom: 0;display:inline-block;")
                                                 .Replace("{ObjectClassifier}", emailMessage.ObjectClassifier)
                                                 .Replace("{Indexer}", emailMessage.Indexer);
            };

            message.Body = builder.ToMessageBody();

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

        public EmailMessageDto GenerateEmailMessage(string receiverEmailAddress, string receiverName, string messageType, string link = null, string objectClassifier = null, string indexer = null)
        {
            EmailMessageDto emailMessage = new EmailMessageDto
            {
                ReceiverName = receiverName,
                ReceiverEmailAddress = receiverEmailAddress,

                Subject = EmailMessageTypesDictionary.EmailMessageSubject[messageType],
                Header = EmailMessageTypesDictionary.EmailMessageHeader[messageType],
                BodyMessage = EmailMessageTypesDictionary.EmailMessageBody[messageType],

                Link = link,
                LinkText = EmailMessageTypesDictionary.EmailMessageLinkText[messageType],

                ObjectClassifier = objectClassifier,
                Indexer = indexer
            };

            return emailMessage;
        }
    }
}


