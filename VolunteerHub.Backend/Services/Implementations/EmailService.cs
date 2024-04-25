using System.Net;
using System.Net.Mail;
using VolunteerHub.Backend.Services.Interfaces;

namespace VolunteerHub.Backend.Services.Implementations
{
    public class EmailService : IEmailService
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            string mail = "vhub.No_reply@outlook.com";
            string pwd = "Parola123!";
            var client = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pwd)
            };
            return client.SendMailAsync(    
                new MailMessage(
                    from: mail,
                    to: email,
                    subject,
                    message
                    )
                );
        }
    }
}
