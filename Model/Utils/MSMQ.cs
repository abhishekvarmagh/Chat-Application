using Experimental.System.Messaging;
using System;
using Model.Models;
using Model.Utils.Interface;

namespace Model.Utils
{
    public class MSMQ : IMSMQ
    {
        private readonly MessageQueue messagQueue;
        public MSMQ()
        {
            this.messagQueue = new MessageQueue();
            this.messagQueue.Path = @".\private$\emailsender";
            if (!MessageQueue.Exists(this.messagQueue.Path))
            {
                this.messagQueue = MessageQueue.Create(this.messagQueue.Path);
            }
        }

        public void Send(string subject, string message, string email)
        {
            this.messagQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(ComposeMail) });
            this.messagQueue.Send(new Models.ComposeMail
            {
                subject = subject,
                message = message,
                email = email
            });
            EmailService.MessageQueue();
        }

        public Message[] ReceiveMsg()
        {
            return messagQueue.GetAllMessages();
        }

    }
}