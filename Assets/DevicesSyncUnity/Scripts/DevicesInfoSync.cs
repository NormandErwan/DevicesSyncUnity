using DevicesSyncUnity.Messages;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace DevicesSyncUnity
{
    /// <summary>
    /// Synchronize static information between devices with <see cref="DeviceInfoMessage"/>.
    /// </summary>
    public class DevicesInfoSync : DevicesSync
    {
        // Properties

        /// <summary>
        /// Gets information from currently connected devices.
        /// </summary>
        public Dictionary<int, DeviceInfoMessage> DevicesInfo { get; protected set; }

        // Events

        /// <summary>
        /// Called on server when a new <see cref="DeviceInfoMessage"/> is received from device.
        /// </summary>
        public event Action<DeviceInfoMessage> ServerDeviceInfoReceived = delegate { };

        /// <summary>
        /// Called on device client when a new <see cref="DeviceInfoMessage"/> is received from another device.
        /// </summary>
        public event Action<DeviceInfoMessage> ClientDeviceInfoReceived = delegate { };

        // Variables

        protected DeviceInfoMessage deviceInfoMessage = new DeviceInfoMessage();

        // Methods

        /// <summary>
        /// Initializes properties.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            MessageTypes.Add(deviceInfoMessage.MessageType);
            DevicesInfo = new Dictionary<int, DeviceInfoMessage>();

            DefaultChannelId = Channels.DefaultReliable;
        }

        /// <summary>
        /// Sends current device information to server when connected.
        /// </summary>
        protected override void Start()
        {
            base.Start();

            deviceInfoMessage.UpdateInfo();
            SendToServer(deviceInfoMessage);
        }

        /// <summary>
        /// Server sends to new device client information from all the currently connected devices. 
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
        {
            var deviceInfoMessage = netMessage.ReadMessage<DeviceInfoMessage>();
            DevicesInfo[deviceInfoMessage.SenderConnectionId] = deviceInfoMessage;
            ServerDeviceInfoReceived.Invoke(deviceInfoMessage);
            return deviceInfoMessage;
        }

        /// <summary>
        /// Device client updates <see cref="DevicesInfo"/> and calls <see cref="ClientDeviceInfoReceived"/>.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
        {
            var deviceInfoMessage = netMessage.ReadMessage<DeviceInfoMessage>();
            DevicesInfo[deviceInfoMessage.SenderConnectionId] = deviceInfoMessage;
            ClientDeviceInfoReceived.Invoke(deviceInfoMessage);
            return deviceInfoMessage;
        }

        /// <summary>
        /// Device client removes the disconnected device from <see cref="DevicesInfo"/>.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        protected override void OnClientDeviceDisconnectedReceived(DeviceInfoMessage deviceInfoMessage)
        {
            DevicesInfo.Remove(deviceInfoMessage.SenderConnectionId);
        }
    }
}
