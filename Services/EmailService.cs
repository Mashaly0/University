using System.Net;
using System.Net.Mail;
using University.Models;

public class EmailService
{
    private readonly EmailSettings _settings;
    public EmailService(EmailSettings settings)
    {
        _settings = settings;
    }

    public void SendEmail(string to, string subject, string body)
    {
        var client = new SmtpClient(_settings.SmtpServer, _settings.Port)
        {
            Credentials = new NetworkCredential(_settings.SenderEmail, _settings.Password),
            EnableSsl = _settings.EnableSSL
        };

        var mail = new MailMessage
        {
            From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mail.To.Add(to);
        client.Send(mail);
    }
}
