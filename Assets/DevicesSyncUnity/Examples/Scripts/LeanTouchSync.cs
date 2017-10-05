using DevicesSyncUnity.Examples.Messages;
using DevicesSyncUnity.Messages;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace DevicesSyncUnity.Examples
{
    public class LeanTouchSync : DevicesSyncInterval
    {
        // Properties

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

        protected bool initialAutoStartSending;
        protected LeanTouchInfoMessage leanTouchInfoMessage = new LeanTouchInfoMessage();
        protected LeanTouchMessage leanTouchMessage = new LeanTouchMessage();
        protected bool lastLeanTouchMessageEmpty = false;

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
        /// Sends a <see cref="LeanTouchInfoMessage"/> to server.
        /// </summary>
        protected override void Start()
        {
            base.Start();

            leanTouchInfoMessage.UpdateInfo();
            SendToServer(leanTouchInfoMessage, Channels.DefaultReliable);
        }

        protected override void OnSendToServerIntervalIteration(bool sendToServerThisFrame)
        {
            leanTouchMessage.UpdateInfo();
            if (sendToServerThisFrame)
            {
                bool emptyLeanTouchMessage = leanTouchMessage.Fingers.Length == 0;
                if (!emptyLeanTouchMessage || !lastLeanTouchMessageEmpty)
                {
                    SendToServer(leanTouchMessage);
                    leanTouchMessage.Reset();
                }
                lastLeanTouchMessageEmpty = emptyLeanTouchMessage;
            }
        }

        protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
        {
            if (netMessage.msgType == leanTouchMessage.MessageType)
            {
                // Get LeanTouch frame information
                var leanTouchMessage = netMessage.ReadMessage<LeanTouchMessage>();
                ServerLeanTouchReceived.Invoke(leanTouchMessage);
                return leanTouchMessage;
            }
            else if (netMessage.msgType == leanTouchInfoMessage.MessageType)
            {
                // Get LeanTouch static information
                var leanTouchInfoMessage = netMessage.ReadMessage<LeanTouchInfoMessage>();
                ServerLeanTouchInfoReceived.Invoke(leanTouchInfoMessage);

                // Send to new device client the LeanTouch static information from other connected devices
                foreach (var leanTouchInfo in LeanTouchInfo)
                {
                    SendToClient(leanTouchInfoMessage.SenderConnectionId, leanTouchInfo.Value);
                }

                LeanTouchInfo[leanTouchInfoMessage.SenderConnectionId] = leanTouchInfoMessage;
                return leanTouchInfoMessage;
            }
            else
            {
                return null;
            }
        }

        protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
        {
            if (netMessage.msgType == leanTouchMessage.MessageType)
            {
                // Get LeanTouch frame information
                var leanTouchMessage = netMessage.ReadMessage<LeanTouchMessage>();
                leanTouchMessage.RestoreInfo(LeanTouchInfo[leanTouchMessage.SenderConnectionId]);

                LeanTouches[leanTouchMessage.SenderConnectionId] = leanTouchMessage;
                ServerLeanTouchReceived.Invoke(leanTouchMessage);
                return leanTouchMessage;
            }
            else if (netMessage.msgType == leanTouchInfoMessage.MessageType)
            {
                // Get LeanTouch static information
                var leanTouchInfoMessage = netMessage.ReadMessage<LeanTouchInfoMessage>();
                LeanTouchInfo[leanTouchInfoMessage.SenderConnectionId] = leanTouchInfoMessage;
                ClientLeanTouchInfoReceived.Invoke(leanTouchInfoMessage);

                // Starts sending LeanTouch frame information as the server has transmited current device's LeanTouch static information
                if (SyncMode != SyncMode.ReceiverOnly && isClient && initialAutoStartSending && !SendingIsStarted)
                {
                    StartSending();
                }

                return leanTouchInfoMessage;
            }
            else
            {
                return null;
            }
        }

        protected override void OnClientDeviceDisconnectedReceived(DeviceInfoMessage deviceInfoMessage)
        {
            LeanTouchInfo.Remove(deviceInfoMessage.SenderConnectionId);
            LeanTouches.Remove(deviceInfoMessage.SenderConnectionId);
        }
    }
}
