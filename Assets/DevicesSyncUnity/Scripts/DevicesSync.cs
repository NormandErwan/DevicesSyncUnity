using DevicesSyncUnity.Messages;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace DevicesSyncUnity
{
    /// <summary>
    /// The different synchronization modes.
    /// </summary>
    public enum SyncMode
    {
        /// <summary>
        /// Device client sends to and receive information from other devices
        /// </summary>
        SenderAndReceiver,

        /// <summary>
        /// Device client only sends its information to other devices.
        /// </summary>
        SenderOnly,

        /// <summary>
        /// Device client only receives information from other devices.
        /// </summary>
        ReceiverOnly
    }

    /// <summary>
    /// Synchronize between connected devices with network messages derived from <see cref="DeviceInfoMessage"/>.
    /// </summary>
    public abstract class DevicesSync : NetworkBehaviour
    {
        // Constants

        /// <summary>
        /// The channel to use for sending messages.
        /// </summary>
        protected static readonly int channelId = Channels.DefaultUnreliable; 

        // Editor fields

        [SerializeField]
        [Tooltip("Synchronization mode to use between device client and the server.")]
        private SyncMode syncMode = SyncMode.SenderAndReceiver;

        // Properties

        /// <summary>
        /// Gets or sets the synchronization mode between device client and the server.
        /// </summary>
        public SyncMode SyncMode { get { return syncMode; } set { syncMode = value; } }

        /// <summary>
        /// Gets the networking message type <see cref="Messages.MessageType"/> to use for exchange between device
        /// client and the server.
        /// </summary>
        protected abstract short MessageType { get; }

        // Events

        /// <summary>
        /// Called in client side when another device has been disconnected from the server.
        /// </summary>
        public static event Action<DeviceInfoMessage> ClientDeviceDisconnected = delegate { };

        // Variables

        protected NetworkManager manager;
        protected uint sendingFrameCounter = 0;
        protected float sendingTimer = 0;

        // Methods

        /// <summary>
        /// Configure the device client and the server to send and receive networking messsages.
        /// </summary>
        protected virtual void Start()
        {
            manager = NetworkManager.singleton;
            if (manager == null)
            {
                UnityEngine.Debug.LogError("There is no NetworkManager in the scene");
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

        /// <summary>
        /// Server broadcasts any received networking message of type <see cref="MessageType"/> to all device clients.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        protected virtual void ServerMessageReceived(NetworkMessage netMessage)
        {
            // Configure the messages
            var message = OnServerReceived(netMessage);
            message.SenderConnectionId = netMessage.conn.connectionId;

            // Send it to all clients
            Utilities.Debug.Log("Server: transfer message (type: " + message.GetType() + ") from device client " 
                + message.SenderConnectionId + " to all device clients", LogFilter.Debug);
            NetworkServer.SendToAll(MessageType, message);
        }

        /// <summary>
        /// Extracts the typed network message.
        /// </summary>
        /// <param name="netMessage">The message received by the server.</param>
        /// <returns>The typed network message extracted.</returns>
        protected abstract DevicesSyncMessage OnServerReceived(NetworkMessage netMessage);

        /// <summary>
        /// Server sends a <see cref="DeviceInfoMessage"/> message to all device clients to inform another device has disconnected.
        /// </summary>
        /// <param name="netMessage">The disconnection message from the disconnected device client.</param>
        protected static void ServerClientDisconnected(NetworkMessage netMessage)
        {
            var deviceInfoMessage = new DeviceInfoMessage();
            deviceInfoMessage.SenderConnectionId = netMessage.conn.connectionId;
            NetworkServer.SendToAll(Messages.MessageType.DeviceDisconnected, deviceInfoMessage);
        }

        /// <summary>
        /// Device client receives a network message of type <see cref="MessageType"/> from the server.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        protected virtual void ClientMessageReceived(NetworkMessage netMessage)
        {
            var message = OnClientReceived(netMessage);
            Utilities.Debug.Log("Client: received message (type: " + message.GetType() + ") from device client " 
                + message.SenderConnectionId, LogFilter.Debug);
        }

        /// <summary>
        /// Extracts the typed network message.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected abstract DevicesSyncMessage OnClientReceived(NetworkMessage netMessage);

        /// <summary>
        /// Device client receives message from server that another device has disconnected.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        protected virtual void ClientDeviceDisconnectedReceived(NetworkMessage netMessage)
        {
            var message = netMessage.ReadMessage<DeviceInfoMessage>();
            OnClientDeviceDisconnectedReceived(message);
            ClientDeviceDisconnected.Invoke(message);
            Utilities.Debug.Log("Client: device client " + message.SenderConnectionId + " disconnected " 
                + MessageType.GetType(), LogFilter.Debug);
        }

        /// <summary>
        /// Device client process the disconnection of another device.
        /// </summary>
        /// <param name="deviceInfoMessage">The received networking message.</param>
        protected abstract void OnClientDeviceDisconnectedReceived(DeviceInfoMessage deviceInfoMessage);

        /// <summary>
        /// Device client sends a message to server.
        /// </summary>
        /// <param name="message">The message to send.</param>
        protected virtual void SendToServer(DevicesSyncMessage message)
        {
            Utilities.Debug.Log("Client: sending message (type: " + message.GetType() + ")", LogFilter.Debug);
            manager.client.SendByChannel(MessageType, message, channelId);
        }

        /// <summary>
        /// Logs the error code.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        protected virtual void OnError(NetworkMessage netMessage)
        {
            var errorMessage = netMessage.ReadMessage<ErrorMessage>();
            Utilities.Debug.LogError("Error: " + errorMessage.errorCode);
        }
    }
}
