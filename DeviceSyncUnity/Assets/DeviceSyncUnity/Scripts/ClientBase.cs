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
        protected string hubName = "Hub";

        [SerializeField]
        protected bool autoStart = true;

        // Event

        public EventHandler Connected = delegate { };

        // Properties

        public HubConnection Connection { get; protected set; }

        public IHubProxy Proxy { get; protected set; }

        public bool IsConnected { get; protected set; }

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
            Proxy = Connection.CreateProxy(hubName);

            connectionThread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                lock (connectionLock)
                {
                    threadConnecting = true;
                }

                Connection.Start();

                lock (connectionLock)
                {
                    IsConnected = true;
                }
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
                if (IsConnected && threadConnecting)
                {
                    threadConnecting = false;
                    Connected.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}