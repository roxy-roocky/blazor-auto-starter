using System;
using main_datacontext;
using Microsoft.Extensions.Logging;

namespace jobcode.Managers.Emails;

public class DbEmailSender : IEmailSender
{
    protected readonly main_datacontext.MainDbContext _dbContext;
    protected readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
    protected readonly ILogger<DbEmailSender> _logger;

    public DbEmailSender(MainDbContext dbContext, Microsoft.Extensions.Configuration.IConfiguration configuration, ILogger<DbEmailSender> logger)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string[] to, string subject, string htmlBody, string? fromName = null, string? fromEmail = null, string[]? cc = null, string[]? bcc = null)
    {
        var dbEmail = new main_datacontext.Emails.DbEmail
        {
            To = string.Join(";", to),
            Cc = cc != null ? string.Join(";", cc) : null,
            Bcc = bcc != null ? string.Join(";", bcc) : null,
            Subject = subject,
            HtmlContent = htmlBody,
            SenderEmail = fromEmail ?? _configuration["Emails:Senders:Default:Email"]!,
            SenderName = fromName ?? _configuration["Emails:Senders:Default:Name"]!,
        };

        try
        {
            await _dbContext.Emails.AddAsync(dbEmail);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email into database");
            throw new IEmailSender.EmailSenderException("Failed to send email", ex);
        }
    }
}
