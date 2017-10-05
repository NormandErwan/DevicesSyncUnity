using DevicesSyncUnity.Messages;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace DevicesSyncUnity
{
    /// <summary>
    /// Synchronize acceleration, device orientation and acceleration events between devices with <see cref="AccelerationMessage"/>.
    /// </summary>
    public class AccelerationSync : DevicesSyncInterval
    {
        // Properties

        /// <summary>
        /// Gets latest acceleration and acceleration events from currently connected devices.
        /// </summary>
        public Dictionary<int, AccelerationMessage> Accelerations { get; protected set; }

        /// <summary>
        /// Gets latest device orientation from currently connected devices.
        /// </summary>
        public Dictionary<int, DeviceOrientationMessage> DeviceOrientations { get; protected set; }

        // Events

        /// <summary>
        /// Called on server when a new <see cref="AccelerationMessage"/> is received from device.
        /// </summary>
        public event Action<AccelerationMessage> ServerAccelerationMessageReceived = delegate { };

        /// <summary>
        /// Called on device client when a new <see cref="AccelerationMessage"/> is received from another device.
        /// </summary>
        public event Action<AccelerationMessage> ClientAccelerationMessageReceived = delegate { };

        /// <summary>
        /// Called on server when a new <see cref="DeviceOrientationMessage"/> is received from device.
        /// </summary>
        public event Action<DeviceOrientationMessage> ServerDeviceOrientationMessageReceived = delegate { };

        /// <summary>
        /// Called on device client when a new <see cref="DeviceOrientationMessage"/> is received from another device.
        /// </summary>
        public event Action<DeviceOrientationMessage> ClientDeviceOrientationMessageReceived = delegate { };

        // Variables

        protected AccelerationMessage accelerationMessage = new AccelerationMessage();
        protected DeviceOrientationMessage deviceOrientationMessage = new DeviceOrientationMessage();

        // Methods

        /// <summary>
        /// Initializes the properties.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            Accelerations = new Dictionary<int, AccelerationMessage>();
            DeviceOrientations = new Dictionary<int, DeviceOrientationMessage>();

            MessageTypes.Add(accelerationMessage.MessageType);
            MessageTypes.Add(deviceOrientationMessage.MessageType);
        }

        /// <summary>
        /// Updates a <see cref="AccelerationMessage"/> and a <see cref="DeviceOrientationMessage"/> and sends them if
        /// required and there are not empty.
        /// </summary>
        /// <param name="sendToServerThisFrame">If the messages should be sent this frame.</param>
        protected override void OnSendToServerIntervalIteration(bool sendToServerThisFrame)
        {
            accelerationMessage.UpdateInfo();
            deviceOrientationMessage.UpdateInfo();

            if (sendToServerThisFrame)
            {
                if (accelerationMessage.accelerationEvents.Length != 0)
                {
                    SendToServer(accelerationMessage);
                    accelerationMessage.Reset();
                }

                if (deviceOrientationMessage.deviceOrientationValues.Length != 0)
                {
                    SendToServer(deviceOrientationMessage);
                    deviceOrientationMessage.Reset();
                }
            }
        }

        /// <summary>
        /// Server invokes <see cref="ServerAccelerationMessageReceived"/> or <see cref="ServerDeviceOrientationMessageReceived"/>. 
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
        {
            if (netMessage.msgType == accelerationMessage.MessageType)
            {
                var accelerationMessage = netMessage.ReadMessage<AccelerationMessage>();
                Accelerations[accelerationMessage.SenderConnectionId] = accelerationMessage;
                ServerAccelerationMessageReceived.Invoke(accelerationMessage);
                return accelerationMessage;
            }
            else if (netMessage.msgType == deviceOrientationMessage.MessageType)
            {
                var deviceOrientationMessage = netMessage.ReadMessage<DeviceOrientationMessage>();
                DeviceOrientations[deviceOrientationMessage.SenderConnectionId] = deviceOrientationMessage;
                ServerDeviceOrientationMessageReceived.Invoke(deviceOrientationMessage);
                return deviceOrientationMessage;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Updates <see cref="Accelerations"/> or <see cref="DeviceOrientations"/> and invokes
        /// <see cref="ClientAccelerationMessageReceived"/> or <see cref="ClientDeviceOrientationMessageReceived"/>.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
        {
            if (netMessage.msgType == accelerationMessage.MessageType)
            {
                var accelerationMessage = netMessage.ReadMessage<AccelerationMessage>();
                Accelerations[accelerationMessage.SenderConnectionId] = accelerationMessage;
                ClientAccelerationMessageReceived.Invoke(accelerationMessage);
                return accelerationMessage;
            }
            else if (netMessage.msgType == deviceOrientationMessage.MessageType)
            {
                var deviceOrientationMessage = netMessage.ReadMessage<DeviceOrientationMessage>();
                DeviceOrientations[deviceOrientationMessage.SenderConnectionId] = deviceOrientationMessage;
                ClientDeviceOrientationMessageReceived.Invoke(deviceOrientationMessage);
                return deviceOrientationMessage;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Removes the disconnected device from <see cref="Accelerations"/> and <see cref="DeviceOrientations"/>.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        protected override void OnClientDeviceDisconnectedReceived(DeviceInfoMessage deviceInfoMessage)
        {
            Accelerations.Remove(deviceInfoMessage.SenderConnectionId);
            DeviceOrientations.Remove(deviceInfoMessage.SenderConnectionId);
        }
    }
}
