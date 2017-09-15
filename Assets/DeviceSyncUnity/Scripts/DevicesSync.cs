using DeviceSyncUnity.Messages;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace DeviceSyncUnity
{
    public enum SyncMode
    {
        SenderAndReceiver,
        SenderOnly,
        ReceiverOnly
    }

    public abstract class DevicesSync : NetworkBehaviour
    {
        // Constants

        protected static readonly int channelId = Channels.DefaultUnreliable; 

        // Editor fields

        [SerializeField]
        private SyncMode syncMode = SyncMode.SenderAndReceiver;

        // Properties

        public SyncMode SyncMode { get { return syncMode; } set { syncMode = value; } }

        protected abstract short MessageType { get; }

        // Events

        public static event Action<DeviceInfoMessage> ClientDeviceDisconnected = delegate { };

        // Variables

        protected NetworkManager manager;
        protected uint sendingFrameCounter = 0;
        protected float sendingTimer = 0;

        // Methods

        protected virtual void Start()
        {
            manager = NetworkManager.singleton;
            if (manager == null)
            {
                Debug.LogError("There is no NetworkManager in the scene");
                return;
            }

            if (isServer)
            {
                NetworkServer.RegisterHandler(MessageType, ServerMessageReceived);
                NetworkServer.RegisterHandler(MsgType.Disconnect, ServerClientDisconnected);
            }

            var client = manager.client;
            if (client != null && isClient)
            {
                if (SyncMode != SyncMode.SenderOnly)
                {
                    client.RegisterHandler(MessageType, ClientMessageReceived);
                    client.RegisterHandler(Messages.MessageType.DeviceDisconnected, ClientDeviceDisconnectedReceived);
                }

                Utilities.Debug.Execute(() => client.RegisterHandler(MsgType.Error, OnError), LogFilter.Error);
            }
        }

        protected virtual void ServerMessageReceived(NetworkMessage netMessage)
        {
            var message = OnServerReceived(netMessage);
            message.SenderConnectionId = netMessage.conn.connectionId;
            Utilities.Debug.Log("Server: transfer message (type: " + message.GetType() + ") from client " + message.SenderConnectionId + " to all clients", LogFilter.Debug);
            NetworkServer.SendToAll(MessageType, message);
        }

        protected abstract DevicesSyncMessage OnServerReceived(NetworkMessage netMessage);

        protected static void ServerClientDisconnected(NetworkMessage netMessage)
        {
            var deviceInfoMessage = new DeviceInfoMessage();
            deviceInfoMessage.SenderConnectionId = netMessage.conn.connectionId;
            NetworkServer.SendToAll(Messages.MessageType.DeviceDisconnected, deviceInfoMessage);
        }

        protected virtual void ClientMessageReceived(NetworkMessage netMessage)
        {
            var message = OnClientReceived(netMessage);
            Utilities.Debug.Log("Client: received message (type: " + message.GetType() + ") from client " + message.SenderConnectionId, LogFilter.Debug);
        }

        protected abstract DevicesSyncMessage OnClientReceived(NetworkMessage netMessage);

        protected virtual void ClientDeviceDisconnectedReceived(NetworkMessage netMessage)
        {
            var message = netMessage.ReadMessage<DeviceInfoMessage>();
            OnClientDeviceDisconnectedReceived(message);
            ClientDeviceDisconnected.Invoke(message);
            Utilities.Debug.Log("Client: client " + message.SenderConnectionId + " disconnected " + MessageType.GetType(), LogFilter.Debug);
        }

        protected abstract void OnClientDeviceDisconnectedReceived(DeviceInfoMessage deviceInfoMessage);

        protected virtual void SendToServer(DevicesSyncMessage message)
        {
            if (!manager.IsClientConnected())
            {
                Utilities.Debug.LogError("Can't send message to server : not connected as client.");
                return;
            }
            Utilities.Debug.Log("Client: sending message (type: " + message.GetType() + ")", LogFilter.Debug);

            manager.client.SendByChannel(MessageType, message, channelId);
        }

        protected virtual void OnError(NetworkMessage netMessage)
        {
            var errorMessage = netMessage.ReadMessage<ErrorMessage>();
            Utilities.Debug.LogError("Error: " + errorMessage.errorCode);
        }
    }
}
