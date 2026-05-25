using System;
using Microsoft.Extensions.Configuration;

namespace jobcode.Managers.Emails;

public interface IEmailSender
{
    public class EmailSenderException(string message, Exception? innerException) : Exception(message, innerException) { }
    public Task SendEmailAsync(string[] to, string subject, string htmlBody, string? fromName = null, string? fromEmail = null, string[]? cc = null, string[]? bcc = null);
}
