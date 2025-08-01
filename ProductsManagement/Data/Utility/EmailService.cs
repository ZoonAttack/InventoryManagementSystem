using Microsoft.AspNetCore.Mvc;
using MimeKit;
using ProductsManagement.DTOs;
using Resend;

namespace ProductsManagement.Data.Utility
{
    public class EmailService
    {
        private readonly IResend _resend;

        private readonly ILogger<EmailService> _logger;

        public EmailService(IResend resend, ILogger<EmailService> logger)
        {
            _resend = resend;
            _logger = logger;
        }


        public async Task<ActionResult<string>> EmailSend(SendEmailDto request)
        {
            var message = new EmailMessage();
            message.From = "onboarding@resend.dev";
            message.To.Add(request.To);
            message.Subject = request.Subject ?? "Hello from EmailService";
            message.TextBody = request.Content;

            var resp = await _resend.EmailSendAsync(message);

            _logger.LogInformation("Sent email to {To}, with Id = {EmailId}", request.To, resp.Content);

            return resp.Content.ToString();
        }

    }
}
