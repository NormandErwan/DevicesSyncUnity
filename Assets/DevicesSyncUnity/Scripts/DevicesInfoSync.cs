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
        /// Called on server and on device client when a new <see cref="DeviceInfoMessage"/> is received.
        /// </summary>
        public event Action<DeviceInfoMessage> DeviceInfoReceived = delegate { };

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
            // Get the information of the new device client
            var deviceInfoMessage = netMessage.ReadMessage<DeviceInfoMessage>();
            DeviceInfoReceived.Invoke(deviceInfoMessage);

            // Send to new device client information from all the currently connected devices
            foreach (var deviceInfo in DevicesInfo)
            {
                SendToClient(deviceInfoMessage.SenderConnectionId, deviceInfo.Value);
            }

            DevicesInfo[deviceInfoMessage.SenderConnectionId] = deviceInfoMessage;
            return deviceInfoMessage;
        }

        /// <summary>
        /// Device client updates <see cref="DevicesInfo"/> and calls <see cref="DeviceInfoReceived"/>.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
        {
            var deviceInfoMessage = netMessage.ReadMessage<DeviceInfoMessage>();
            DevicesInfo[deviceInfoMessage.SenderConnectionId] = deviceInfoMessage;
            DeviceInfoReceived.Invoke(deviceInfoMessage);
            return deviceInfoMessage;
        }

        /// <summary>
        /// Device client removes the disconnected device from <see cref="DevicesInfo"/>.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        protected override void OnClientDeviceDisconnectedReceived(DeviceDisconnectedMessage deviceDisconnectedMessage)
        {
            DevicesInfo.Remove(deviceDisconnectedMessage.SenderConnectionId);
        }
    }
}
