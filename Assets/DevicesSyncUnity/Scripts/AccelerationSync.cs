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
        /// Gets latest accelerations and acceleration events from currently connected devices.
        /// </summary>
        public Dictionary<int, AccelerationMessage> Accelerations { get; protected set; }

        /// <summary>
        /// Gets latest device orientations from currently connected devices.
        /// </summary>
        public Dictionary<int, DeviceOrientationMessage> DeviceOrientations { get; protected set; }

        // Events

        /// <summary>
        /// Called on server and on device client when a new <see cref="AccelerationMessage"/> is received.
        /// </summary>
        public event Action<AccelerationMessage> AccelerationMessageReceived = delegate { };

        /// <summary>
        /// Called on server and on device client when a new <see cref="DeviceOrientationMessage"/> is received.
        /// </summary>
        public event Action<DeviceOrientationMessage> DeviceOrientationMessageReceived = delegate { };

        // Variables

        protected AccelerationMessage accelerationMessage = new AccelerationMessage();
        protected DeviceOrientationMessage deviceOrientationMessage = new DeviceOrientationMessage();

        // Methods

        /// <summary>
        /// Initializes the properties and susbcribes to events.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            Accelerations = new Dictionary<int, AccelerationMessage>();
            DeviceOrientations = new Dictionary<int, DeviceOrientationMessage>();

            DeviceDisconnected += DevicesInfoSync_DeviceDisconnected;

            MessageTypes.Add(accelerationMessage.MessageType);
            MessageTypes.Add(deviceOrientationMessage.MessageType);
        }

        /// <summary>
        /// Unsubscribes to events.
        /// </summary>
        protected virtual void OnDestroy()
        {
            DeviceDisconnected -= DevicesInfoSync_DeviceDisconnected;
        }

        /// <summary>
        /// Updates a <see cref="AccelerationMessage"/> and a <see cref="DeviceOrientationMessage"/> and sends them if
        /// required and there are not empty.
        /// </summary>
        /// <param name="sendToServerThisFrame">If the messages should be sent this frame.</param>
        protected override void OnSendToServerIntervalIteration(bool sendToServerThisFrame)
        {
            accelerationMessage.Update();
            deviceOrientationMessage.Update();

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
        /// Calls <see cref="ProcessMessageReceived(NetworkMessage)"/>.
        /// </summary>
        protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
        {
            if (netMessage.msgType == accelerationMessage.MessageType)
            {
                var accelerationMessage = netMessage.ReadMessage<AccelerationMessage>();
                ProcessMessageReceived(accelerationMessage);
                return accelerationMessage;
            }
            else if (netMessage.msgType == deviceOrientationMessage.MessageType)
            {
                var deviceOrientationMessage = netMessage.ReadMessage<DeviceOrientationMessage>();
                ProcessMessageReceived(deviceOrientationMessage);
                return deviceOrientationMessage;
            }
            return null;
        }

        /// <summary>
        /// Calls <see cref="ProcessMessageReceived(NetworkMessage)"/>.
        /// </summary>
        protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
        {
            if (netMessage.msgType == accelerationMessage.MessageType)
            {
                var accelerationMessage = netMessage.ReadMessage<AccelerationMessage>();
                if (!isServer)
                {
                    ProcessMessageReceived(accelerationMessage);
                }
                return accelerationMessage;
            }
            else if (netMessage.msgType == deviceOrientationMessage.MessageType)
            {
                var deviceOrientationMessage = netMessage.ReadMessage<DeviceOrientationMessage>();
                if (!isServer)
                {
                    ProcessMessageReceived(deviceOrientationMessage);
                }
                return deviceOrientationMessage;
            }
            return null;
        }

        /// <summary>
        /// Removes the disconnected device from <see cref="Accelerations"/> and <see cref="DeviceOrientations"/>.
        /// </summary>
        /// <param name="deviceId">The id of the disconnected device.</param>
        protected virtual void DevicesInfoSync_DeviceDisconnected(int deviceId)
        {
            Accelerations.Remove(deviceId);
            DeviceOrientations.Remove(deviceId);
        }

        /// <summary>
        /// Updates <see cref="Accelerations"/> or <see cref="DeviceOrientations"/> and invokes
        /// <see cref="AccelerationMessageReceived"/> or <see cref="DeviceOrientationMessageReceived"/>.
        /// </summary>
        /// <param name="accelerationMessage">The received message.</param>
        protected virtual void ProcessMessageReceived(AccelerationMessage accelerationMessage)
        {
            Accelerations[accelerationMessage.SenderConnectionId] = accelerationMessage;
            AccelerationMessageReceived.Invoke(accelerationMessage);
        }

        /// <summary>
        /// Updates <see cref="DeviceOrientations"/> and invokes <see cref="DeviceOrientationMessageReceived"/>.
        /// </summary>
        /// <param name="deviceOrientationMessage">The received message.</param>
        protected virtual void ProcessMessageReceived(DeviceOrientationMessage deviceOrientationMessage)
        {
            DeviceOrientations[deviceOrientationMessage.SenderConnectionId] = deviceOrientationMessage;
            DeviceOrientationMessageReceived.Invoke(deviceOrientationMessage);
        }
    }
}
