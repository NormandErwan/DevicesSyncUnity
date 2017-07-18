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
                Log("Device synchronization for Unity - Middleware");
                Log("Running on " + url);
                Log("Press Esc to exit");

                while (Console.ReadKey(true).Key != ConsoleKey.Escape) { }
            }
        }

        internal static void Log(string message)
        {
            Console.WriteLine(message);
        }
    }

    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }
}
