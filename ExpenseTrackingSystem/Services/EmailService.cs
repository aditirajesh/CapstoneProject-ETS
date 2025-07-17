using System;
using System.Net;
using System.Net.Mail;
using ExpenseTrackingSystem.Interfaces;

namespace ExpenseTrackingSystem.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }
    public async Task SendReportEmailAsync(string recipientEmail, string subject, string body, byte[]? attachment = null, string? attachmentName = null)
    {
        try
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_configuration["Smtp:Username"]), 
                Body = body,
                IsBodyHtml = false
            };
            message.To.Add(new MailAddress(recipientEmail)); 


            if (attachment != null && !string.IsNullOrEmpty(attachmentName))
            {
                var stream = new MemoryStream(attachment);
                message.Attachments.Add(new Attachment(stream, attachmentName, "text/csv"));
            }

            var smtpClient = new SmtpClient
            {
                Host = _configuration["Smtp:Host"],
                Port = int.Parse(_configuration["Smtp:Port"]),
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    _configuration["Smtp:Username"], 
                    _configuration["Smtp:Password"])
            };

            await smtpClient.SendMailAsync(message);
            _logger.LogInformation("Email sent to {Recipient}", recipientEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipient}", recipientEmail);
            throw;
        }
    }

}
