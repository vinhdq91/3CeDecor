using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace DAL.UnitOfWorks.MailService
{
    public class SmtpSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public interface IMailer
    {
        Task SendEmailAsync(string email, string subject, string body);
    }
}
