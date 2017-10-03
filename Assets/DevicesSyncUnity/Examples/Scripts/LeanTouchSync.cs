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
        private LeanTouch leanTouch;

        // Properties

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

        public LeanTouch LeanTouch { get { return leanTouch; } set { leanTouch = value; } }

        public Dictionary<int, LeanTouchMessage> LeanTouches { get; protected set; }

        /// <summary>
        /// See <see cref="DevicesSync.MessageType"/>.
        /// </summary>
        protected override short MessageType { get { return Messages.MessageType.LeanTouch; } }

        // Variables

        protected LeanTouchMessage leanTouchMessage = new LeanTouchMessage();

        // Events

        /// <summary>
        /// Called on server when a new <see cref="LeanTouchMessage"/> is received from device.
        /// </summary>
        public event Action<LeanTouchMessage> ServerLeanTouchReceived = delegate { };

        /// <summary>
        /// Called on device client when a new <see cref="LeanTouchMessage"/> is received from another device.
        /// </summary>
        public event Action<LeanTouchMessage> ClientLeanTouchReceived = delegate { };

        // Methods

        /// <summary>
        /// Initializes the properties.
        /// </summary>
        protected virtual void Awake()
        {
            LeanTouches = new Dictionary<int, LeanTouchMessage>();
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
            var leanTouchMessage = netMessage.ReadMessage<LeanTouchMessage>();
            ServerLeanTouchReceived.Invoke(leanTouchMessage);
            return leanTouchMessage;
        }

        protected override DevicesSyncMessage OnClientReceived(NetworkMessage netMessage)
        {
            var leanTouchMessage = netMessage.ReadMessage<LeanTouchMessage>();
            leanTouchMessage.RestoreInfo();

            LeanTouches[leanTouchMessage.SenderConnectionId] = leanTouchMessage;
            ServerLeanTouchReceived.Invoke(leanTouchMessage);
            return leanTouchMessage;
        }

        protected override void OnClientDeviceDisconnectedReceived(DeviceInfoMessage deviceInfoMessage)
        {
            LeanTouches.Remove(deviceInfoMessage.SenderConnectionId);
        }
    }
}
