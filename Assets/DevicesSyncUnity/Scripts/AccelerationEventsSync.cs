using DevicesSyncUnity.Messages;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace DevicesSyncUnity
{
    /// <summary>
    /// Synchronize acceleration events between devices with <see cref="AccelerationEventsMessage"/>.
    /// </summary>
    public class AccelerationEventsSync : DevicesSyncInterval
    {
        // Properties

        /// <summary>
        /// Gets latest acceleration events from currently connected devices.
        /// </summary>
        public Dictionary<int, AccelerationEventsMessage> AccelerationEvents { get; protected set; }

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

        protected AccelerationEventsMessage accelerationEventsMessage = new AccelerationEventsMessage();
        protected bool zeroAccelerationLastMessage = false;

        // Methods

        /// <summary>
        /// Initializes properties.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            AccelerationEvents = new Dictionary<int, AccelerationEventsMessage>();
            MessageTypes.Add(accelerationEventsMessage.MessageType);
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
            AccelerationEvents[accelerationMessage.SenderConnectionId] = accelerationMessage;
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
