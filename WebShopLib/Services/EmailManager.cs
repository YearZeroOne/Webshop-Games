using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;

namespace WebShopLib.Services
{
    public static class EmailManager
    {
        /// <summary>
        /// Sends an email to the specified address with the given subject and body.
        /// </summary>
        /// <param name="email">The email address to send the message to.</param>
        /// <param name="subject">The subject of the message.</param>
        /// <param name="body">The body of the message.</param>
        public static void SendEmail(string email, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse("ap14webshop@outlook.com"));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Plain) { Text = body };



            using (var client = new SmtpClient())
            {
                client.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate("ap14webshop@outlook.com", "Akt14webshop#");
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
