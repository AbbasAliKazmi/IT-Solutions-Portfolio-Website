using System.Net;
using System.Net.Mail;

namespace ProductAPI.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _user;
        private readonly string _pass;
        private readonly string _fromEmail;

        public SmtpEmailSender(IConfiguration config)
        {
            _host = config["Smtp:Host"] ?? throw new ArgumentNullException("Smtp:Host");
            _port = int.Parse(config["Smtp:Port"] ?? "587");
            _user = config["Smtp:User"] ?? throw new ArgumentNullException("Smtp:User");
            _pass = config["Smtp:Pass"] ?? throw new ArgumentNullException("Smtp:Pass");
            _fromEmail = config["Smtp:FromEmail"] ?? throw new ArgumentNullException("Smtp:FromEmail");
        }

        public async Task SendEmailAsync(string to, string subject, string htmlMessage)
        {
            using var client = new SmtpClient(_host, _port)
            {
                Credentials = new NetworkCredential(_user, _pass),
                EnableSsl = true
            };

            var mailMessage = new MailMessage(_fromEmail, to, subject, htmlMessage)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mailMessage);
        }
    }
}
