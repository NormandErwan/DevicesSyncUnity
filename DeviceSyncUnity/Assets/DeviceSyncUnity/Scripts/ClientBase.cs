using Newtonsoft.Json;
using SignalR.Client._20.Hubs;
using System;
using System.Threading;
using UnityEngine;

namespace DeviceSyncUnity
{
    public abstract class ClientBase : MonoBehaviour
    {
        // Editor fields

        [SerializeField]
        protected string hubConnectionUrl = "http://localhost:8080";

        [SerializeField]
        protected string hubName = "DeviceSyncHub";

        [SerializeField]
        protected bool autoStart = true;

        // Event

        public EventHandler ConnectionStarted = delegate { };

        // Properties

        public HubConnection Connection { get; protected set; }

        public IHubProxy Proxy { get; protected set; }

        // Variables

        protected Thread connectionThread;

        protected object connectionLock = new object();

        protected bool threadConnecting;

        // Methods

        public void ConnectionStartAsync()
        {
            connectionThread.Start();
        }

        protected virtual void Awake()
        {
            Connection = new HubConnection(hubConnectionUrl);
            Proxy = Connection.CreateProxy(hubName, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            connectionThread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                lock (connectionLock)
                {
                    threadConnecting = true;
                }

                Connection.Start();
            });
        }

        protected virtual void Start()
        {
            if (autoStart)
            {
                ConnectionStartAsync();
            }
        }

        protected virtual void Update()
        {
            lock (connectionLock)
            {
                if (threadConnecting && Connection.IsStarted)
                {
                    threadConnecting = false;
                    ConnectionStarted.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}