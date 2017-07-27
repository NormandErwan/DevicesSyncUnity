using Microsoft.AspNet.SignalR.Hubs;
using System;

namespace DeviceSyncUnity.Middleware
{
    public class LoggingPipelineModule : HubPipelineModule
    {
        public static void Log(string message)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss") + " - " + message);
        }

        protected override bool OnBeforeConnect(IHub hub)
        {
            Log("Connecting client " + hub.Context.ConnectionId + " on hub " + hub.ToString());
            return base.OnBeforeConnect(hub);
        }
        protected override bool OnBeforeDisconnect(IHub hub, bool stopCalled)
        {
            Log("Disconnecting client " + hub.Context.ConnectionId + " on hub " + hub.ToString());
            return base.OnBeforeDisconnect(hub, stopCalled);
        }

        protected override bool OnBeforeIncoming(IHubIncomingInvokerContext context)
        {
            Log("=> Invoking " + context.MethodDescriptor.Name + " on hub " + context.MethodDescriptor.Hub.Name);
            return base.OnBeforeIncoming(context);
        }

        protected override bool OnBeforeOutgoing(IHubOutgoingInvokerContext context)
        {
            Log("<= Invoking " + context.Invocation.Method + " on hub " + context.Invocation.Hub);
            return base.OnBeforeOutgoing(context);
        }
    }
}
