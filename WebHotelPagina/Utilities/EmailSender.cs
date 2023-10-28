using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace SlnWeb.Utilities
{
    public class EmailSender : IEmailSender
    {

        public string clave { get; set; }
        public EmailSender(IConfiguration _config)
        {
            clave = _config.GetValue<string>("SendGrid:SecretKey");
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var cliente = new SendGridClient(clave);
            var from = new EmailAddress("i202113647@cibertec.edu.pe");
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);
            return cliente.SendEmailAsync(msg);
        }
    }
}
