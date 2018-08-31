using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models.Manager
{
    public class MailManager
    {
        public void sendEmail(string to, string subject, string body)
        {
            Helper helper = new Helper();
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials =
                new System.Net.NetworkCredential(helper.DecodeFrom64("ZHVjcGExOTk2QGdtYWlsLmNvbQ=="),
                helper.DecodeFrom64("ZHVjMTIzNDUNCg=="));
            MailMessage mm = new MailMessage(helper.DecodeFrom64("ZHVjcGExOTk2QGdtYWlsLmNvbQ == "), to);
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            mm.Subject = subject;
            mm.Body = body;
            mm.IsBodyHtml = true;
            client.Send(mm);
        }
    }
}
