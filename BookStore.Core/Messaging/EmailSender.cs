using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using static BookStore.Common.GlobalExceptions;

namespace BookStore.Core.Messaging
{
    public class EmailSender : IEmailSender
    {
        private readonly SendGridClient client;
        private readonly ILogger<EmailSender> logger;

        public EmailSender(
            string apiKey,
            ILogger<EmailSender> _logger)
        {
            this.client = new SendGridClient(apiKey);
            logger = _logger;
        }


        public async Task SendEmailAsync(string from, string fromName, string to, string subject, string htmlContent, IEnumerable<EmailAttachment>
            attachments = null)
        {
            var message = CheckParameters(from, fromName, to, subject, htmlContent, attachments);

            try
            {
                var response = await this.client.SendEmailAsync(message);
                logger.LogInformation(response.StatusCode.ToString());
                logger.LogInformation(await response.Body.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(SendEmailAsync), ex);

                throw new ApplicationException(EmailSendFailed, ex);
            }
        }

        private SendGridMessage CheckParameters(string from, string fromName, string to, string subject, string htmlContent, IEnumerable<EmailAttachment>
            attachments = null)
        {
            if (string.IsNullOrWhiteSpace(subject) && string.IsNullOrWhiteSpace(htmlContent))
            {
                throw new ArgumentException(SubjectAndMessageShouldBeProvided);
            }

            var fromAddress = new EmailAddress(from, fromName);
            var toAddress = new EmailAddress(to);
            var message = MailHelper.CreateSingleEmail(fromAddress, toAddress, subject, null, htmlContent);

            if (attachments?.Any() == true)
            {
                foreach (var attachment in attachments)
                {
                    message.AddAttachment(attachment.FileName, Convert.ToBase64String(attachment.Content), attachment.MimeType);
                }
            }

            return message;
        }
    }
}
