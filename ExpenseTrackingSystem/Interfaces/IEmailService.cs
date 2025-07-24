using System;

namespace ExpenseTrackingSystem.Interfaces;

public interface IEmailService
{
    Task SendReportEmailAsync(string recipientEmail, string subject, string body, byte[]? attachment = null, string? attachmentName = null);
    Task SendSuggestionEmailAsync(string recipientEmail, string analyserUsername, string suggestionContent, string reportPeriod);

}
