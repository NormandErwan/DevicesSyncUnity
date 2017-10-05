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
        /// Called on server and device client when a <see cref="LeanTouchInfoMessage"/> is received.
        /// </summary>
        public event Action<LeanTouchInfoMessage> LeanTouchInfoReceived = delegate { };

        /// <summary>
        /// Called on server and device client when a <see cref="LeanTouchMessage"/> is received.
        /// </summary>
        public event Action<LeanTouchMessage> LeanTouchReceived = delegate { };

        /// <summary>
        /// Called on server and device client for every <see cref="LeanTouchMessage.FingersDown"/> when received.
        /// </summary>
        public event Action<int, LeanFingerInfo> OnFingerDown = delegate { };

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

            leanTouchMessage.SetCapturingEvents(true);

            leanTouchInfoMessage.UpdateInfo();
            SendToServer(leanTouchInfoMessage, Channels.DefaultReliable);
        }

        public override void OnNetworkDestroy()
        {
            base.OnNetworkDestroy();
            leanTouchMessage.SetCapturingEvents(false);
        }

        protected override void OnSendToServerIntervalIteration(bool sendToServerThisFrame)
        {
            if (sendToServerThisFrame)
            {
                leanTouchMessage.UpdateInfo();

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
                return ProcessLeanTouchMessage(netMessage);
            }
            else if (netMessage.msgType == leanTouchInfoMessage.MessageType)
            {
                var leanTouchInfoMessage = netMessage.ReadMessage<LeanTouchInfoMessage>();
                foreach (var leanTouchInfo in LeanTouchInfo)
                {
                    SendToClient(leanTouchInfoMessage.SenderConnectionId, leanTouchInfo.Value);
                }

                LeanTouchInfo[leanTouchInfoMessage.SenderConnectionId] = leanTouchInfoMessage;
                LeanTouchInfoReceived.Invoke(leanTouchInfoMessage);
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
                return ProcessLeanTouchMessage(netMessage);
            }
            else if (netMessage.msgType == leanTouchInfoMessage.MessageType)
            {
                var leanTouchInfoMessage = netMessage.ReadMessage<LeanTouchInfoMessage>();
                LeanTouchInfo[leanTouchInfoMessage.SenderConnectionId] = leanTouchInfoMessage;
                LeanTouchInfoReceived.Invoke(leanTouchInfoMessage);
                
                if (SyncMode != SyncMode.ReceiverOnly && isClient && initialAutoStartSending && !SendingIsStarted)
                {
                    StartSending(); // Starts sending LeanTouchMessage as the server has received LeanTouchInfoMessage
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

        protected virtual LeanTouchMessage ProcessLeanTouchMessage(NetworkMessage netMessage)
        {
            var leanTouchMessage = netMessage.ReadMessage<LeanTouchMessage>();
            leanTouchMessage.RestoreInfo(LeanTouchInfo[leanTouchMessage.SenderConnectionId]);

            LeanTouches[leanTouchMessage.SenderConnectionId] = leanTouchMessage;
            LeanTouchReceived.Invoke(leanTouchMessage);

            foreach (var fingerDown in leanTouchMessage.FingersDown)
            {
                OnFingerDown.Invoke(leanTouchMessage.SenderConnectionId, fingerDown);
            }

            return leanTouchMessage;
        }
    }
}
