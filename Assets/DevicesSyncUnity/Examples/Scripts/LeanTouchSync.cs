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
        [Tooltip("Interval mode to use to send regularly messages.")]
        private SendingMode sendingMode = SendingMode.TimeInterval;

        [SerializeField]
        [Tooltip("The number of frame to use between each message in FramesInterval mode.")]
        private float sendingTimeInterval = 0.1f;

        [SerializeField]
        [Tooltip("The time in seconds to use between each message in TimeInterval mode.")]
        private uint sendingFramesInterval = 1;

        [SerializeField]
        [Tooltip("The LeanTouch instance to synchronize with other devices.")]
        private LeanTouch leanTouch;

        // Properties

        /// <summary>
        /// See <see cref="DevicesSync.MessageTypes"/>.
        /// </summary>
        protected override List<short> MessageTypes { get { return messageTypes; } }

        /// <summary>
        /// See <see cref="DevicesSyncInterval.SendingMode"/>.
        /// </summary>
        public override SendingMode SendingMode { get { return sendingMode; } set { sendingMode = value; } }

        /// <summary>
        /// See <see cref="DevicesSyncInterval.SendingTimeInterval"/>.
        /// </summary>
        public override float SendingTimeInterval { get { return sendingTimeInterval; } set { sendingTimeInterval = value; } }

        /// <summary>
        /// See <see cref="DevicesSyncInterval.SendingFramesInterval"/>.
        /// </summary>
        public override uint SendingFramesInterval { get { return sendingFramesInterval; } set { sendingFramesInterval = value; } }

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
        protected List<short> messageTypes = new List<short>();

        // Methods

        /// <summary>
        /// Initializes the properties.
        /// </summary>
        protected virtual void Awake()
        {
            LeanTouchInfo = new Dictionary<int, LeanTouchInfoMessage>();
            LeanTouches = new Dictionary<int, LeanTouchMessage>();

            messageTypes.Add(leanTouchMessage.MessageType);
            messageTypes.Add(leanTouchInfoMessage.MessageType);
        }

        /// <summary>
        /// Sends LeanTouch static information to server when connected.
        /// </summary>
        protected override void Start()
        {
            base.Start();

            leanTouchInfoMessage.UpdateInfo();
            SendToServer(leanTouchInfoMessage, Channels.DefaultReliable);
        }

        protected override void OnSendToServerIntervalIteration(bool sendToServerThisFrame)
        {
            if (sendToServerThisFrame)
            {
                leanTouchMessage.UpdateInfo();
                SendToServer(leanTouchMessage);
            }
        }

        protected override DevicesSyncMessage OnServerReceived(NetworkMessage netMessage)
        {
            if (netMessage.msgType == leanTouchMessage.MessageType)
            {
                var leanTouchReceived = netMessage.ReadMessage<LeanTouchMessage>();
                ServerLeanTouchReceived.Invoke(leanTouchReceived);
                return leanTouchMessage;
            }
            else if (netMessage.msgType == leanTouchInfoMessage.MessageType)
            {
                var leanTouchInfoReceived = netMessage.ReadMessage<LeanTouchInfoMessage>();
                ServerLeanTouchInfoReceived.Invoke(leanTouchInfoReceived);

                int clientConnectionId = netMessage.conn.connectionId;
                foreach (var leanTouchInfo in LeanTouchInfo)
                {
                    SendToClient(clientConnectionId, leanTouchInfo.Value);
                }

                return leanTouchInfoReceived;
            }
            else
            {
                return null; // TODO: throw exception?
            }
        }

        protected override DevicesSyncMessage OnClientReceived(NetworkMessage netMessage)
        {
            if (netMessage.msgType == leanTouchMessage.MessageType)
            {
                var leanTouchReceived = netMessage.ReadMessage<LeanTouchMessage>();
                leanTouchReceived.RestoreInfo(LeanTouchInfo[leanTouchReceived.SenderConnectionId]);

                LeanTouches[leanTouchReceived.SenderConnectionId] = leanTouchReceived;
                ServerLeanTouchReceived.Invoke(leanTouchReceived);
                return leanTouchReceived;
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
                return null; // TODO: throw exception?
            }
        }

        protected override void OnClientDeviceDisconnectedReceived(DeviceInfoMessage deviceInfoMessage)
        {
            LeanTouchInfo.Remove(deviceInfoMessage.SenderConnectionId);
            LeanTouches.Remove(deviceInfoMessage.SenderConnectionId);
        }
    }
}
