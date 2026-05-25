using System;
using html_generator;
using jobcode.Managers.Emails;
using main_datacontext.Identity;
using Microsoft.AspNetCore.Identity;

namespace webapp.Managers.Account;

public class EmailManager : IEmailSender<main_datacontext.Identity.User>
{
    protected readonly ILogger<EmailManager> _logger;
    protected readonly jobcode.Managers.Emails.IEmailSender _emailSender;
    protected readonly html_generator.HtmlRendererService _htmlRendererService;

    public EmailManager(IEmailSender emailSender, ILogger<EmailManager> logger, HtmlRendererService htmlRendererService)
    {
        _emailSender = emailSender;
        _logger = logger;
        _htmlRendererService = htmlRendererService;
    }

    public async Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
    {
        await _emailSender.SendEmailAsync(
            to: [email],
            subject: "Lien de confirmation de création de compte",
            htmlBody: await _htmlRendererService.RenderComponentAsync<html_generator.Emails.Account.Confirmation>(parameters =>
            {
                parameters.Add("User", user);
                parameters.Add("ConfirmationLink", confirmationLink);
            })
        );
    }

    public async Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
    {
        await _emailSender.SendEmailAsync(
            to: [email],
            subject: "Code de réinitialisation de mot de passe",
            htmlBody: await _htmlRendererService.RenderComponentAsync<html_generator.Emails.Account.PasswordReset>(parameters =>
            {
                parameters.Add("User", user);
                parameters.Add("ResetCode", resetCode);
            })
        );
    }

    public async Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
    {
        await _emailSender.SendEmailAsync(
            to: [email],
            subject: "Lien de réinitialisation de mot de passe",
            htmlBody: await _htmlRendererService.RenderComponentAsync<html_generator.Emails.Account.PasswordResetLink>(parameters =>
            {
                parameters.Add("User", user);
                parameters.Add("ResetLink", resetLink);
            })
        );
    }
}
