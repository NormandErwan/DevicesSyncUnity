using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace DeviceSyncUnity.Middleware
{
    public class DeviceSyncHub : Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.AddMessage(name, message);
        }

        public void CallMethodAllClients(string method, dynamic data)
        {
            IClientProxy proxy = Clients.All;
            proxy.Invoke(method, data);
        }

        public void CallMethodOtherClients(string method, dynamic data)
        {
            IClientProxy proxy = Clients.Others;
            proxy.Invoke(method, data);
        }
    }
}
