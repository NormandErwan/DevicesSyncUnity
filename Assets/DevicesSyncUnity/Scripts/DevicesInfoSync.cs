using DevicesSyncUnity.Messages;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace DevicesSyncUnity
{
    /// <summary>
    /// Synchronize static information between devices with <see cref="DevicesInfoMessage"/>.
    /// </summary>
    public class DevicesInfoSync : DevicesSync
    {
        // Properties

        /// <summary>
        /// Gets information from currently connected devices.
        /// </summary>
        public Dictionary<int, DevicesInfoMessage> DevicesInfo { get; protected set; }

        /// <summary>
        /// See <see cref="DevicesSync.MessageType"/>.
        /// </summary>
        protected override short MessageType { get { return Messages.MessageType.DeviceInfo; } }

        // Events

        /// <summary>
        /// Called on server when a new <see cref="DevicesInfoMessage"/> is received from device.
        /// </summary>
        public event Action<DevicesInfoMessage> ServerDeviceInfoReceived = delegate { };

        /// <summary>
        /// Called on device client when a new <see cref="DevicesInfoMessage"/> is received from another device.
        /// </summary>
        public event Action<DevicesInfoMessage> ClientDeviceInfoReceived = delegate { };

        // Methods

        /// <summary>
        /// Initializes properties.
        /// </summary>
        protected virtual void Awake()
        {
            DevicesInfo = new Dictionary<int, DevicesInfoMessage>();
        }

        /// <summary>
        /// Sends current device information to server when connected.
        /// </summary>
        protected override void Start()
        {
            base.Start();

            var deviceInfoMessage = new DevicesInfoMessage();
            deviceInfoMessage.UpdateInfo();
            SendToServer(deviceInfoMessage);
        }

        /// <summary>
        /// Server sends to new device client information from all the currently connected devices. 
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected override DevicesSyncMessage OnServerReceived(NetworkMessage netMessage)
        {
            var deviceInfoMessage = netMessage.ReadMessage<DevicesInfoMessage>();
            ServerDeviceInfoReceived.Invoke(deviceInfoMessage);

            int clientConnectionId = netMessage.conn.connectionId;
            foreach (var deviceInfo in DevicesInfo)
            {
                Utilities.Debug.Log("Server: transfer " + deviceInfo.Value.GetType() + " from client " 
                    + deviceInfo.Value.SenderConnectionId + " to client " + clientConnectionId, LogFilter.Debug);
                NetworkServer.SendToClient(clientConnectionId, MessageType, deviceInfo.Value);
            }

            return deviceInfoMessage;
        }

        /// <summary>
        /// Device client updates <see cref="DevicesInfo"/> and calls <see cref="ClientDeviceInfoReceived"/>.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected override DevicesSyncMessage OnClientReceived(NetworkMessage netMessage)
        {
            var deviceInfoMessage = netMessage.ReadMessage<DevicesInfoMessage>();
            DevicesInfo[deviceInfoMessage.SenderConnectionId] = deviceInfoMessage;
            ClientDeviceInfoReceived.Invoke(deviceInfoMessage);
            return deviceInfoMessage;
        }

        /// <summary>
        /// Device client removes the disconnected device from <see cref="DevicesInfo"/>.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        protected override void OnClientDeviceDisconnectedReceived(DevicesInfoMessage deviceInfoMessage)
        {
            DevicesInfo.Remove(deviceInfoMessage.SenderConnectionId);
        }
    }
}
