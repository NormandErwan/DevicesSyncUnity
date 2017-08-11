using System;
using UnityEngine;

namespace DeviceSyncUnity
{
    public class DeviceSyncClient : MonoBehaviour
    {
        // Editor fields

        [SerializeField]
        protected string serverIp = "localhost";

        [SerializeField]
        protected int serverPort = 7777;

        [SerializeField]
        protected bool autoStartConnection = true;

        // Event

        public EventHandler ConnectionStarted = delegate { };

        // Properties

        public string ServerIp { get { return serverIp; } set { serverIp = value; } }

        public int ServerPort { get { return serverPort; } set { serverPort = value; } }

        public bool AutoStartConnection { get { return autoStartConnection; } set { autoStartConnection = value; } }

        // Methods

        protected virtual void Start()
        {
            if (AutoStartConnection)
            {
            }
        }
    }
}
