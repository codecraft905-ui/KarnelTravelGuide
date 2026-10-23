
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace KarnelTravelGuide.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        var host = _config["EmailSettings:Host"];
        var username = _config["EmailSettings:Username"];
        var senderEmail = _config["EmailSettings:SenderEmail"];
        // Google shows App Passwords in 4 groups with spaces; strip them so either form works.
        var appPassword = (_config["EmailSettings:AppPassword"] ?? "").Replace(" ", "").Trim();

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(senderEmail))
            throw new InvalidOperationException("EmailSettings (Host / Username / SenderEmail) are missing in appsettings.json.");
        if (appPassword.Length == 0)
            throw new InvalidOperationException(
                "EmailSettings:AppPassword is empty. Set it with: dotnet user-secrets set \"EmailSettings:AppPassword\" \"<16-character app password>\" " +
                "(User Secrets are only loaded when ASPNETCORE_ENVIRONMENT=Development).");
        if (!int.TryParse(_config["EmailSettings:Port"], out var port)) port = 587;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_config["EmailSettings:SenderName"], senderEmail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var smtp = new SmtpClient { Timeout = 20000 };
        await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(username, appPassword);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }
}
