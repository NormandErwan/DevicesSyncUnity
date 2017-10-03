using DevicesSyncUnity.Messages;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DevicesSyncUnity
{
    /// <summary>
    /// Synchronize acceleration events between devices with <see cref="AccelerationEventsMessage"/>.
    /// </summary>
    public class AccelerationEventsSync : DevicesSyncInterval
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
        private uint sendingFramesInterval = 2;

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

        /// <summary>
        /// Gets latest acceleration events from currently connected devices.
        /// </summary>
        public Dictionary<int, AccelerationEventsMessage> AccelerationEvents { get; protected set; }

        /// <summary>
        /// See <see cref="DevicesSync.MessageTypes"/>.
        /// </summary>
        protected override List<short> MessageTypes { get { return messageTypes; } }

        // Events

        /// <summary>
        /// Called on server when a new <see cref="AccelerationEventsMessage"/> is received from device.
        /// </summary>
        public event Action<AccelerationEventsMessage> ServerAccelerationEventsReceived = delegate { };

        /// <summary>
        /// Called on device client when a new <see cref="AccelerationEventsMessage"/> is received from another device.
        /// </summary>
        public event Action<AccelerationEventsMessage> ClientAccelerationEventsReceived = delegate { };

        // Variables

        protected List<short> messageTypes = new List<short>() { new AccelerationEventsMessage().MessageType };
        protected AccelerationEventsMessage accelerationEventsMessage = new AccelerationEventsMessage();
        protected bool zeroAccelerationLastMessage = false;

        // Methods

        /// <summary>
        /// Initializes properties.
        /// </summary>
        protected virtual void Awake()
        {
            AccelerationEvents = new Dictionary<int, AccelerationEventsMessage>();
        }

        /// <summary>
        /// Sends current and previous frames acceleration events if required and if there are acceleration events and
        /// there were acceleration events at the previous interval.
        /// </summary>
        /// <param name="sendToServerThisFrame">If the acceleration events should be sent this frame.</param>
        protected override void OnSendToServerIntervalIteration(bool sendToServerThisFrame)
        {
            accelerationEventsMessage.UpdateInfo();

            bool zeroAcceleration  = (accelerationEventsMessage.accelerationEvents.Length == 0 
                || accelerationEventsMessage.accelerationEvents[0].acceleration.sqrMagnitude == 0f);
            if (sendToServerThisFrame && !(zeroAcceleration && zeroAccelerationLastMessage))
            {
                SendToServer(accelerationEventsMessage);
                accelerationEventsMessage.Reset();
            }
            zeroAccelerationLastMessage = zeroAcceleration;
        }

        /// <summary>
        /// Server invokes <see cref="ServerAccelerationEventsReceived"/>. 
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected override DevicesSyncMessage OnServerReceived(NetworkMessage netMessage)
        {
            var accelerationMessage = netMessage.ReadMessage<AccelerationEventsMessage>();
            ServerAccelerationEventsReceived.Invoke(accelerationMessage);
            return accelerationMessage;
        }

        /// <summary>
        /// Device client updates <see cref="AccelerationEvents"/> and calls <see cref="ClientAccelerationEventsReceived"/>.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected override DevicesSyncMessage OnClientReceived(NetworkMessage netMessage)
        {
            var accelerationMessage = netMessage.ReadMessage<AccelerationEventsMessage>();
            AccelerationEvents[accelerationMessage.SenderConnectionId] = accelerationMessage;
            ClientAccelerationEventsReceived.Invoke(accelerationMessage);
            return accelerationMessage;
        }

        /// <summary>
        /// Device client removes the disconnected device from <see cref="AccelerationEvents"/>.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        protected override void OnClientDeviceDisconnectedReceived(DeviceInfoMessage deviceInfoMessage)
        {
            AccelerationEvents.Remove(deviceInfoMessage.SenderConnectionId);
        }
    }
}
