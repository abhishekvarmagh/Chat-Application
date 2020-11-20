using Experimental.System.Messaging;
using System;
using System.Net;
using System.Net.Mail;
using Model.Utils.Interface;
using Model.Models;

namespace Model.Utils
{
    public class EmailService
    {
        private static IMSMQ msmq = new Utils.MSMQ();

        public static void NotifyThroughMail(string toEmail, string subject, string message)
        {
            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress("countrybookshop@gmail.com");
            mailMessage.Subject = subject;
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = true;
            mailMessage.To.Add(new MailAddress(toEmail));

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";

            smtp.EnableSsl = true;
            NetworkCredential networkCredential = new NetworkCredential();
            networkCredential.UserName = mailMessage.From.Address;
            networkCredential.Password = "RuntimeTerror@123";
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = networkCredential;
            smtp.Port = 587;
            smtp.Send(mailMessage);
        }

        public static void MessageQueue()
        {
            Message[] messages = msmq.ReceiveMsg();
            foreach (Message message in messages)
            {
                message.Formatter = new XmlMessageFormatter(new Type[] { typeof(ComposeMail) });
                ComposeMail composeMail = (ComposeMail)message.Body;
                NotifyThroughMail(composeMail.email, composeMail.message, composeMail.email);
            }
        }
    }
}
