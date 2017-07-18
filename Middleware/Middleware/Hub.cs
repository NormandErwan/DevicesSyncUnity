using System;

namespace DeviceSyncUnity.Middleware
{
    public class Hub : Microsoft.AspNet.SignalR.Hub
    {
        public void Send(string name, string message)
        {
            Program.Log("send");
            Clients.All.AddMessage(name, message);
        }

        public dynamic RequestReplyDynamic()
        {
            return new { time = DateTime.Now.ToLongTimeString() };
        }

        public int RequestReplyValueType()
        {
            return DateTime.Now.Millisecond;
        }
    }
}
