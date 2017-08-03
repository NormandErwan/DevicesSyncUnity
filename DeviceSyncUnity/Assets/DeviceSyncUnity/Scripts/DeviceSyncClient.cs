using Newtonsoft.Json;
using SignalR.Client._20.Hubs;
using System;
using System.Threading;
using UnityEngine;

namespace DeviceSyncUnity
{
    public class DeviceSyncClient : MonoBehaviour
    {
        // Editor fields

        [SerializeField]
        protected string hubConnectionUrl = "http://localhost:8090";

        [SerializeField]
        protected string hubName = "DeviceSyncHub";

        [SerializeField]
        protected bool autoStartConnection = true;

        // Event

        public EventHandler ConnectionStarted = delegate { };

        // Properties

        public string HubConnectionUrl { get { return hubConnectionUrl; } set { hubConnectionUrl = value; } }

        public string HubName { get { return hubName; } set { hubName = value; } }

        public bool AutoStartConnection { get { return autoStartConnection; } set { autoStartConnection = value; } }

        public HubConnection Connection { get; protected set; }

        public IHubProxy Proxy { get; protected set; }

        // Variables

        protected Thread connectionThread;

        protected object connectionLock = new object();

        protected bool threadConnecting;

        // Methods

        public void CreateConnection()
        {
            Logger.Log("Connection created");

            Connection = new HubConnection(HubConnectionUrl);
            Proxy = Connection.CreateProxy(HubName, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        public void ConnectionStartAsync()
        {
            Logger.Log("Connection starting");
            connectionThread.Start();
        }

        public void ConnectionCancel()
        {
            // TODO: update the SignalR code
        }

        protected virtual void Awake()
        {
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
            if (AutoStartConnection)
            {
                CreateConnection();
                ConnectionStartAsync();
            }
        }

        protected virtual void Update()
        {
            lock (connectionLock)
            {
                if (threadConnecting && Connection.IsStarted)
                {
                    Logger.Log("Connection started");

                    threadConnecting = false;
                    ConnectionStarted.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
