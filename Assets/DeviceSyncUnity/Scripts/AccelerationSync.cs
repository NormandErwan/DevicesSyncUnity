using DeviceSyncUnity.Messages;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DeviceSyncUnity
{
    /// <summary>
    /// Synchronize acceleration events between devices with <see cref="AccelerationMessage"/>.
    /// </summary>
    public class AccelerationSync : DevicesSyncInterval
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
        public Dictionary<int, AccelerationMessage> Accelerations { get; protected set; }

        /// <summary>
        /// See <see cref="DevicesSync.MessageType"/>.
        /// </summary>
        protected override short MessageType { get { return Messages.MessageType.Acceleration; } }

        // Variables

        protected AccelerationMessage accelerationMessage = new AccelerationMessage();
        protected bool zeroAccelerationLastMessage = false;

        // Events

        /// <summary>
        /// Called on server when a new <see cref="AccelerationMessage"/> is received from device.
        /// </summary>
        public event Action<AccelerationMessage> ServerAccelerationReceived = delegate { };

        /// <summary>
        /// Called on device client when a new <see cref="AccelerationMessage"/> is received from another device.
        /// </summary>
        public event Action<AccelerationMessage> ClientAccelerationReceived = delegate { };

        // Methods

        /// <summary>
        /// Initializes properties.
        /// </summary>
        protected virtual void Awake()
        {
            Accelerations = new Dictionary<int, AccelerationMessage>();
        }

        /// <summary>
        /// Sends current and previous frames acceleration events if required and if there are acceleration events and
        /// there were acceleration events at the previous interval.
        /// </summary>
        /// <param name="sendToServerThisFrame">If the acceleration events should be sent this frame.</param>
        protected override void OnSendToServerIntervalIteration(bool sendToServerThisFrame)
        {
            accelerationMessage.UpdateInfo();

            bool zeroAcceleration  = (accelerationMessage.accelerationEvents.Length == 0 
                || accelerationMessage.accelerationEvents[0].acceleration.sqrMagnitude == 0f);
            if (sendToServerThisFrame && !(zeroAcceleration && zeroAccelerationLastMessage))
            {
                SendToServer(accelerationMessage);
                accelerationMessage = new AccelerationMessage();
            }
            zeroAccelerationLastMessage = zeroAcceleration;
        }

        /// <summary>
        /// Server invokes <see cref="ServerAccelerationReceived"/>. 
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected override DevicesSyncMessage OnServerReceived(NetworkMessage netMessage)
        {
            var accelerationMessage = netMessage.ReadMessage<AccelerationMessage>();
            ServerAccelerationReceived.Invoke(accelerationMessage);
            return accelerationMessage;
        }

        /// <summary>
        /// Device client updates <see cref="Accelerations"/> and calls <see cref="ClientAccelerationReceived"/>.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected override DevicesSyncMessage OnClientReceived(NetworkMessage netMessage)
        {
            var accelerationMessage = netMessage.ReadMessage<AccelerationMessage>();
            Accelerations[accelerationMessage.SenderConnectionId] = accelerationMessage;
            ClientAccelerationReceived.Invoke(accelerationMessage);
            return accelerationMessage;
        }

        /// <summary>
        /// Device client removes the disconnected device from <see cref="Accelerations"/>.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        protected override void OnClientDeviceDisconnectedReceived(DeviceInfoMessage deviceInfoMessage)
        {
            Accelerations.Remove(deviceInfoMessage.SenderConnectionId);
        }
    }
}
