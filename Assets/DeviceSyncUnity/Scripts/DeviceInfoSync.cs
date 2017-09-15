using DeviceSyncUnity.Messages;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace DeviceSyncUnity
{
    public class DeviceInfoSync : DevicesSync
    {
        // Properties

        public Dictionary<int, DeviceInfoMessage> DevicesInfo { get; protected set; }

        protected override short MessageType { get { return Messages.MessageType.DeviceInfo; } }

        // Events

        public event Action<DeviceInfoMessage> ClientDeviceInfoReceived = delegate { };

        // Methods

        protected virtual void Awake()
        {
            DevicesInfo = new Dictionary<int, DeviceInfoMessage>();
        }

        protected override void Start()
        {
            base.Start();

            var deviceInfoMessage = new DeviceInfoMessage();
            deviceInfoMessage.UpdateInfo();
            SendToServer(deviceInfoMessage);
        }

        protected override DevicesSyncMessage OnServerReceived(NetworkMessage netMessage)
        {
            var deviceInfoMessage = netMessage.ReadMessage<DeviceInfoMessage>();

            int clientConnectionId = netMessage.conn.connectionId;
            foreach (var deviceInfo in DevicesInfo)
            {
                Utilities.Debug.Log("Server: transfer " + deviceInfo.Value.GetType() + " from client " + deviceInfo.Value.SenderConnectionId + " to client " + clientConnectionId, LogFilter.Debug);
                NetworkServer.SendToClient(clientConnectionId, MessageType, deviceInfo.Value);
            }

            return deviceInfoMessage;
        }

        protected override DevicesSyncMessage OnClientReceived(NetworkMessage netMessage)
        {
            var deviceInfoMessage = netMessage.ReadMessage<DeviceInfoMessage>();
            DevicesInfo[deviceInfoMessage.SenderConnectionId] = deviceInfoMessage;
            ClientDeviceInfoReceived.Invoke(deviceInfoMessage);
            return deviceInfoMessage;
        }

        protected override void OnClientDeviceDisconnectedReceived(DeviceInfoMessage deviceInfoMessage)
        {
            DevicesInfo.Remove(deviceInfoMessage.SenderConnectionId);
        }
    }
}
