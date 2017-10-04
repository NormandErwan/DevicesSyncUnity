using DevicesSyncUnity.Examples.Messages;
using DevicesSyncUnity.Messages;
using Lean.Touch;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DevicesSyncUnity.Examples
{
    /// <summary>
    /// Synchronize acceleration events between devices with <see cref="AccelerationEventsMessage"/>.
    /// </summary>
    public class LeanTouchSync : DevicesSyncInterval
    {
        // Editor fields

        [SerializeField]
        [Tooltip("The LeanTouch instance to synchronize with other devices.")]
        private LeanTouch leanTouch;

        // Properties

        /// <summary>
        /// Gets or sets the LeanTouch instance to synchronize with other devices.
        /// </summary>
        public LeanTouch LeanTouch { get { return leanTouch; } set { leanTouch = value; } }

        /// <summary>
        /// Gets LeanTouch static information from currently connected devices.
        /// </summary>
        public Dictionary<int, LeanTouchInfoMessage> LeanTouchInfo { get; protected set; }

        /// <summary>
        /// Gets latest LeanTouch information from currently connected devices.
        /// </summary>
        public Dictionary<int, LeanTouchMessage> LeanTouches { get; protected set; }

        // Events

        /// <summary>
        /// Called on server when a new <see cref="LeanTouchInfoMessage"/> is received from device.
        /// </summary>
        public event Action<LeanTouchInfoMessage> ServerLeanTouchInfoReceived = delegate { };

        /// <summary>
        /// Called on device client when a new <see cref="LeanTouchInfoMessage"/> is received from another device.
        /// </summary>
        public event Action<LeanTouchInfoMessage> ClientLeanTouchInfoReceived = delegate { };

        /// <summary>
        /// Called on server when a new <see cref="LeanTouchMessage"/> is received from device.
        /// </summary>
        public event Action<LeanTouchMessage> ServerLeanTouchReceived = delegate { };

        /// <summary>
        /// Called on device client when a new <see cref="LeanTouchMessage"/> is received from another device.
        /// </summary>
        public event Action<LeanTouchMessage> ClientLeanTouchReceived = delegate { };

        // Variables

        protected LeanTouchMessage leanTouchMessage = new LeanTouchMessage();
        protected LeanTouchInfoMessage leanTouchInfoMessage = new LeanTouchInfoMessage();
        protected bool initialAutoStartSending;

        // Methods

        /// <summary>
        /// Initializes the properties.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            initialAutoStartSending = AutoStartSending;
            AutoStartSending = false;

            LeanTouchInfo = new Dictionary<int, LeanTouchInfoMessage>();
            LeanTouches = new Dictionary<int, LeanTouchMessage>();

            MessageTypes.Add(leanTouchInfoMessage.MessageType);
            MessageTypes.Add(leanTouchMessage.MessageType);
        }

        /// <summary>
        /// Sends LeanTouch static information to server when connected.
        /// </summary>
        protected override void Start()
        {
            base.Start();

            leanTouchInfoMessage.UpdateInfo();
            SendToServer(leanTouchInfoMessage, Channels.DefaultReliable);

            if (initialAutoStartSending)
            {
                StartSending();
            }
        }

        protected override void OnSendToServerIntervalIteration(bool sendToServerThisFrame)
        {
            if (sendToServerThisFrame)
            {
                leanTouchMessage.UpdateInfo();
                SendToServer(leanTouchMessage);
            }
        }

        protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
        {
            if (netMessage.msgType == leanTouchMessage.MessageType)
            {
                var leanTouchReceived = netMessage.ReadMessage<LeanTouchMessage>();
                ServerLeanTouchReceived.Invoke(leanTouchReceived);
                return leanTouchReceived;
            }
            else if (netMessage.msgType == leanTouchInfoMessage.MessageType)
            {
                var leanTouchInfoReceived = netMessage.ReadMessage<LeanTouchInfoMessage>();
                LeanTouchInfo[leanTouchInfoReceived.SenderConnectionId] = leanTouchInfoReceived;
                ServerLeanTouchInfoReceived.Invoke(leanTouchInfoReceived);
                return leanTouchInfoReceived;
            }
            else
            {
                return null; // TODO: throw exception?
            }
        }

        protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
        {
            if (netMessage.msgType == leanTouchMessage.MessageType)
            {
                var leanTouchReceived = netMessage.ReadMessage<LeanTouchMessage>();
                int senderId = leanTouchReceived.SenderConnectionId;
                if (LeanTouchInfo.ContainsKey(senderId))
                {
                    leanTouchReceived.RestoreInfo(LeanTouchInfo[leanTouchReceived.SenderConnectionId]);

                    LeanTouches[leanTouchReceived.SenderConnectionId] = leanTouchReceived;
                    ServerLeanTouchReceived.Invoke(leanTouchReceived);
                    return leanTouchReceived;
                }
            }
            else if (netMessage.msgType == leanTouchInfoMessage.MessageType)
            {
                var leanTouchInfoReceived = netMessage.ReadMessage<LeanTouchInfoMessage>();
                LeanTouchInfo[leanTouchInfoReceived.SenderConnectionId] = leanTouchInfoReceived;
                ClientLeanTouchInfoReceived.Invoke(leanTouchInfoReceived);
                return leanTouchInfoReceived;
            }
            else
            {
                // TODO: throw exception?
            }
            return null;
        }

        protected override void OnClientDeviceDisconnectedReceived(DeviceInfoMessage deviceInfoMessage)
        {
            LeanTouchInfo.Remove(deviceInfoMessage.SenderConnectionId);
            LeanTouches.Remove(deviceInfoMessage.SenderConnectionId);
        }
    }
}
