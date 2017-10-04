using DevicesSyncUnity.Messages;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace DevicesSyncUnity
{
    /// <summary>
    /// Synchronize acceleration and acceleration events between devices with <see cref="AccelerationMessage"/>.
    /// </summary>
    public class AccelerationSync : DevicesSyncInterval
    {
        // Properties

        /// <summary>
        /// Gets latest acceleration and acceleration events from currently connected devices.
        /// </summary>
        public Dictionary<int, AccelerationMessage> Accelerations { get; protected set; }

        // Events

        /// <summary>
        /// Called on server when a new <see cref="AccelerationMessage"/> is received from device.
        /// </summary>
        public event Action<AccelerationMessage> ServerAccelerationEventsReceived = delegate { };

        /// <summary>
        /// Called on device client when a new <see cref="AccelerationMessage"/> is received from another device.
        /// </summary>
        public event Action<AccelerationMessage> ClientAccelerationEventsReceived = delegate { };

        // Variables

        protected AccelerationMessage accelerationMessage = new AccelerationMessage();

        // Methods

        /// <summary>
        /// Initializes the properties.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            Accelerations = new Dictionary<int, AccelerationMessage>();
            MessageTypes.Add(accelerationMessage.MessageType);
        }

        /// <summary>
        /// Sends the acceleration events to the server that occured since the latest send or keep them in reference
        /// for a future send.
        /// </summary>
        /// <param name="sendToServerThisFrame">If the acceleration events should be sent this frame.</param>
        protected override void OnSendToServerIntervalIteration(bool sendToServerThisFrame)
        {
            accelerationMessage.UpdateInfo();
            if (sendToServerThisFrame && accelerationMessage.AccelerationEvents.Count != 0)
            {
                SendToServer(accelerationMessage);
                accelerationMessage.Reset();
            }
        }

        /// <summary>
        /// Server invokes <see cref="ServerAccelerationEventsReceived"/>. 
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
        {
            var accelerationMessage = netMessage.ReadMessage<AccelerationMessage>();
            Accelerations[accelerationMessage.SenderConnectionId] = accelerationMessage;
            ServerAccelerationEventsReceived.Invoke(accelerationMessage);
            return accelerationMessage;
        }

        /// <summary>
        /// Device client updates <see cref="Accelerations"/> and invokes <see cref="ClientAccelerationEventsReceived"/>.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
        {
            var accelerationMessage = netMessage.ReadMessage<AccelerationMessage>();
            Accelerations[accelerationMessage.SenderConnectionId] = accelerationMessage;
            ClientAccelerationEventsReceived.Invoke(accelerationMessage);
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
