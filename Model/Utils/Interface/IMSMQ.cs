using Experimental.System.Messaging;

namespace Model.Utils.Interface
{
    public interface IMSMQ
    {
        void Send(string subject, string message, string email);

        Message[] ReceiveMsg();
    }
}
