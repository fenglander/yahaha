

using MailKit.Net.Smtp;
using MimeKit;


namespace Yahaha.Plugin.CommonTool;
/// <summary>
/// MailKit数据库相关基础服务
/// </summary>
/// 
public interface IMailKitService
{
    Task SendEmailAsync(string Body, bool isHtml, string[]? toEmails, string? subject);
}

public class MailKitService : IMailKitService
{
    MailKitOptions Options = App.GetOptions<MailKitOptions>();
    public async Task SendEmailAsync(string Body, bool isHtml, string[]? toEmails, string? subject)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(Options.DefaultFromName, Options.DefaultFromEmail));

        if (toEmails != null && toEmails.Length > 0)
        {
            foreach (var toEmail in toEmails)
            {
                message.To.Add(new MailboxAddress("", toEmail));
            }
        }
        else
        {
            message.To.Add(new MailboxAddress("", Options.DefaultToEmail));
        }

        message.Subject = subject != null ? subject : Options.DefaultFromName;

        var builder = new BodyBuilder();
        if (isHtml)
        {

            builder.HtmlBody = Body;
        }
        else
        {
            builder.TextBody = Body;
        }
        message.Body = builder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(Options.Host, Options.Port, Options.EnableSsl);
            await client.AuthenticateAsync(Options.UserName, Options.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}

