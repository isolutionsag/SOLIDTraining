﻿using System.Collections.Generic;
using System.Linq;

namespace LetterORight
{
    public class MailingLogic
    {
        private readonly IEmailAddressFileReader _emailAddressFileReader;
        private readonly IEmailAddressValidator _emailAddressValidator;
        private readonly IEmailSender _emailSender;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object"></see> class.</summary>
        public MailingLogic(IEmailSender emailSender = null, IEmailAddressValidator emailAddressValidator = null, IEmailAddressFileReader emailAddressFileReader = null)
        {
            _emailAddressFileReader = emailAddressFileReader ?? new EmailAddressFileReader();
            _emailAddressValidator = emailAddressValidator ?? new EmailAddressValidator();
            _emailSender = emailSender ?? new EmailSender();
        }

        public void ProcessMailing(string filepath)
        {
            var addresses = GetEmailAddresses(filepath);

            foreach (var emailAddress in addresses.Where(a => _emailAddressValidator.IsValid(a)))
            {
                var mailContent = GetMailContentFor(emailAddress);
                SendEmail(emailAddress, mailContent);
            }
        }

        protected virtual void SendEmail(string emailAddress, MailContent mailContent)
        {
            _emailSender.SendMail(emailAddress, mailContent.Subject, mailContent.Body);
        }

        protected virtual IEnumerable<string> GetEmailAddresses(string filepath)
        {
            return _emailAddressFileReader.ReadMailAddressesFromFile(filepath);
        }

        protected virtual MailContent GetMailContentFor(string recipient)
        {
            if (recipient.Contains("@example.com"))
            {
                return new MailContent
                {
                    Subject = "example: Hello SOLID",
                    Body = "Our guests from example are the most contacted people in the world!"
                };
            }
            if (recipient.Contains("@example1.com"))
            {
                return new MailContent
                {
                    Subject = "example1: Hello SOLID",
                    Body = "Our guests from example1 have some special text!"
                };
            }
            return new MailContent
            {
                Subject = "Default: Hello SOLID",
                Body = "This is the default content body for hello SOLID"
            };
        }

        protected struct MailContent
        {
            public string Subject { get; set; }
            public string Body { get; set; }
        }
    }
}