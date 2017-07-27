using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using System;

namespace DeviceSyncUnity.Middleware
{
    class Program
    {
        private const string url = "http://*:8080";
        private const int mainLoopEndDelay = 10;

        static void Main(string[] args)
        {
            using (WebApp.Start<Startup>(url))
            {
                LoggingPipelineModule.Log("Device synchronization for Unity - Middleware");
                LoggingPipelineModule.Log("Running on " + url);

                Console.WriteLine("\nPress Esc to exit\n");

                while (Console.ReadKey(true).Key != ConsoleKey.Escape) { }
            }
        }
    }

    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalHost.HubPipeline.AddModule(new LoggingPipelineModule());
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }
}
