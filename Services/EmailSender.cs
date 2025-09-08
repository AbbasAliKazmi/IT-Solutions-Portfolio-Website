using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace ProductAPI.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlMessage)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_config["Smtp:FromName"], _config["Smtp:From"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };
            email.Body = body;

            using var client = new SmtpClient();
            var host = _config["Smtp:Host"];
            var port = int.Parse(_config["Smtp:Port"] ?? "25");
            var enableSsl = bool.Parse(_config["Smtp:EnableSsl"] ?? "false");

            await client.ConnectAsync(host, port, enableSsl);

            var user = _config["Smtp:User"];
            var pass = _config["Smtp:Pass"];
            if (!string.IsNullOrEmpty(user))
            {
                await client.AuthenticateAsync(user, pass);
            }

            await client.SendAsync(email);
            await client.DisconnectAsync(true);
        }
    }
}
