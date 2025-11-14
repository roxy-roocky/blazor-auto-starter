using System;
using brevo_csharp.Api;
using jobcode.Managers.Emails;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace jobcode.Managers.Emails;

public class BrevoEmailSender: IEmailSender
{
    protected readonly IConfiguration _configuration;
    protected readonly ILogger<BrevoEmailSender> _logger;

    private TransactionalEmailsApi _transactionalEmailsApi;
    public BrevoEmailSender(IConfiguration configuration, ILogger<BrevoEmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;

        _transactionalEmailsApi = new TransactionalEmailsApi(new brevo_csharp.Client.Configuration()
        {
            ApiKey = { { "api-key", _configuration["Emails:Brevo:ApiKey"] } },
        });
        
    }

    public async Task SendEmailAsync(string[] to, string subject, string htmlBody, string? fromName = null, string? fromEmail = null, string[]? cc = null, string[]? bcc = null)
    {
        var message = new brevo_csharp.Model.SendSmtpEmail()
        {
            To = [.. to.Select(email => new brevo_csharp.Model.SendSmtpEmailTo(email))],
            Cc = cc?.Select(email => new brevo_csharp.Model.SendSmtpEmailCc(email)).ToList(),
            Bcc = bcc?.Select(email => new brevo_csharp.Model.SendSmtpEmailBcc(email)).ToList(),
            Sender = new brevo_csharp.Model.SendSmtpEmailSender(
                fromName ?? _configuration["Emails:Senders:Default:Name"], 
                fromEmail ?? _configuration["Emails:Senders:Default:Email"]
            ),
            Subject = subject,
            HtmlContent = htmlBody,
        };

        try
        {
            await _transactionalEmailsApi.SendTransacEmailAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email with Brevo");
            throw new IEmailSender.EmailSenderException("Failed to send email with Brevo", ex);
        }
    }
}
