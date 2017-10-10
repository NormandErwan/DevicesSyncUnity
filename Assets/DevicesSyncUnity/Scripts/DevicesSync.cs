using DevicesSyncUnity.Messages;
using System;
using System.Collections.Generic;
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
    /// Synchronize between connected devices with network messages derived from <see cref="DevicesSyncMessage"/>.
    /// </summary>
    public abstract class DevicesSync : NetworkBehaviour
    {
        // Editor fields

        [SerializeField]
        [Tooltip("The network manager to use for receive messages from the server.")]
        protected NetworkManager networkManager;

        [SerializeField]
        [Tooltip("Synchronization mode to use between device clients and the server.")]
        private SyncMode syncMode = SyncMode.SenderAndReceiver;

        // Properties

        /// <summary>
        /// The network manager to use for receive messages from the server.
        /// </summary>
        public NetworkManager NetworkManager { get { return networkManager; } set { networkManager = value; } }

        /// <summary>
        /// Gets or sets the synchronization mode between device clients and the server.
        /// </summary>
        public SyncMode SyncMode { get { return syncMode; } set { syncMode = value; } }

        /// <summary>
        /// Gets the list of ids of currently connected devices to the server.
        /// </summary>
        public List<int> ConnectedDeviceIds { get; protected set; }

        /// <summary>
        /// Gets the default channel to use for sending messages.
        /// </summary>
        protected virtual int DefaultChannelId { get { return defaultChannelId; } set { defaultChannelId = value; } }

        /// <summary>
        /// Gets the networking message types to use for exchange between device clients and the server.
        /// </summary>
        protected List<short> MessageTypes { get; set; }

        // Events

        /// <summary>
        /// Called when a device has connected to the server.
        /// </summary>
        public static event Action<int> DeviceConnected = delegate { };

        /// <summary>
        /// Called when a device has been disconnected from the server.
        /// </summary>
        public static event Action<int> DeviceDisconnected = delegate { };

        // Variables

        private static Action<int> onClientDeviceDisconnected = delegate { };
        private DeviceDisconnectedMessage deviceDisconnectedMessage = new DeviceDisconnectedMessage();
        private int defaultChannelId = Channels.DefaultUnreliable;

        // Methods

        /// <summary>
        /// Initializes the properties.
        /// </summary>
        protected virtual void Awake()
        {
            ConnectedDeviceIds = new List<int>();
            MessageTypes = new List<short>();
            onClientDeviceDisconnected += OnClientDeviceDisconnected;
        }

        protected virtual void OnDestroy()
        {
            onClientDeviceDisconnected -= OnClientDeviceDisconnected;
        }

        /// <summary>
        /// Configures the device client and the server to send and receive networking messsages.
        /// </summary>
        protected virtual void Start()
        {
            NetworkManager = NetworkManager.singleton;
            if (NetworkManager == null)
            {
                throw new Exception("There is no NetworkManager in the scene");
            }

            if (isServer)
            {
                NetworkServer.RegisterHandler(MsgType.Disconnect, ServerClientDisconnected);
                foreach (var messageType in MessageTypes)
                {
                    NetworkServer.RegisterHandler(messageType, ServerMessageReceived);
                }
            }

            var client = NetworkManager.client;
            if (client != null && isClient)
            {
                if (SyncMode != SyncMode.SenderOnly)
                {
                    client.RegisterHandler(deviceDisconnectedMessage.MessageType, ClientDeviceDisconnectedReceived);
                    foreach (var messageType in MessageTypes)
                    {
                        client.RegisterHandler(messageType, ClientMessageReceived);
                    }
                }

                if (LogFilter.logError)
                {
                    client.RegisterHandler(MsgType.Error, OnError);
                }
            }
        }

        /// <summary>
        /// Server broadcasts any received networking message of type <see cref="MessageTypes"/> to all device clients.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        protected virtual void ServerMessageReceived(NetworkMessage netMessage)
        {
            var message = OnServerMessageReceived(netMessage);
            if (message != null)
            {
                if (!ConnectedDeviceIds.Contains(message.SenderConnectionId))
                {
                    if (LogFilter.logInfo)
                    {
                        UnityEngine.Debug.Log("Server: device client " + message.SenderConnectionId + " has connected");
                    }

                    ConnectedDeviceIds.Add(message.SenderConnectionId);
                    ConnectedDeviceIds.Sort();

                    if (isClient)
                    {
                        OnClientDeviceConnected(message.SenderConnectionId);
                    }
                    DeviceConnected.Invoke(message.SenderConnectionId);
                }
                SendToAllClients(message);
            }
            else
            {
                if (LogFilter.logWarn)
                {
                    UnityEngine.Debug.Log("OnServerMessageReceived has returned null instead of returning a "
                    + "DevicesSyncMessage read from the received NetworkMessage");
                }
            }
        }

        /// <summary>
        /// Extracts the typed network message.
        /// </summary>
        /// <param name="netMessage">The message received by the server.</param>
        /// <returns>The typed network message extracted.</returns>
        protected abstract DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage);

        /// <summary>
        /// Server sends a <see cref="DeviceInfoMessage"/> message to all device clients to inform another device has disconnected.
        /// </summary>
        /// <param name="netMessage">The disconnection message from the disconnected device client.</param>
        protected void ServerClientDisconnected(NetworkMessage netMessage)
        {
            var connectionId = netMessage.conn.connectionId;
            if (ConnectedDeviceIds.Contains(connectionId))
            {
                if (LogFilter.logInfo)
                {
                    UnityEngine.Debug.Log("Server: device client " + connectionId + " has disconnected");
                }
                ConnectedDeviceIds.Remove(connectionId);

                deviceDisconnectedMessage.SenderConnectionId = connectionId;
                DeviceDisconnected.Invoke(deviceDisconnectedMessage.SenderConnectionId);
                SendToAllClients(deviceDisconnectedMessage, Channels.DefaultReliable);
            }
        }

        /// <summary>
        /// Device client receives a network message of type <see cref="MessageTypes"/> from the server.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        protected virtual void ClientMessageReceived(NetworkMessage netMessage)
        {
            var message = OnClientMessageReceived(netMessage);
            if (message != null)
            {
                if (!ConnectedDeviceIds.Contains(message.SenderConnectionId))
                {
                    if (LogFilter.logInfo)
                    {
                        UnityEngine.Debug.Log("Client: device client " + message.SenderConnectionId + " has connected");
                    }

                    ConnectedDeviceIds.Add(message.SenderConnectionId);
                    ConnectedDeviceIds.Sort();
                    OnClientDeviceConnected(message.SenderConnectionId);
                    DeviceConnected.Invoke(message.SenderConnectionId);
                }

                if (LogFilter.logInfo)
                {
                    UnityEngine.Debug.Log("Client: received message from device client " + message.SenderConnectionId
                    + ": " + message);
                }
            }
            else
            {
                if (LogFilter.logWarn)
                {
                    UnityEngine.Debug.Log("OnClientMessageReceived has returned null instead of returning a "
                    + "DevicesSyncMessage read from the received NetworkMessage");
                }
            }
        }

        /// <summary>
        /// Extracts the typed network message.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected abstract DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage);

        /// <summary>
        /// Device client process the connection of another device.
        /// </summary>
        /// <param name="deviceId">The id of the new connected device.</param>
        protected virtual void OnClientDeviceConnected(int deviceId)
        {
            if (!ConnectedDeviceIds.Contains(deviceId))
            {
                ConnectedDeviceIds.Add(deviceId);
            }
        }

        /// <summary>
        /// Device client receives message from server that another device has disconnected.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        protected static void ClientDeviceDisconnectedReceived(NetworkMessage netMessage)
        {
            var message = netMessage.ReadMessage<DeviceDisconnectedMessage>();
            if (LogFilter.logInfo)
            {
                UnityEngine.Debug.Log("Client: device client " + message.SenderConnectionId + " has disconnected");
            }

            onClientDeviceDisconnected.Invoke(message.SenderConnectionId);
            DeviceDisconnected.Invoke(message.SenderConnectionId);
        }

        /// <summary>
        /// Device client process the disconnection of another device.
        /// </summary>
        /// <param name="deviceId">The id of the disconnected device.</param>
        protected virtual void OnClientDeviceDisconnected(int deviceId)
        {
            if (ConnectedDeviceIds.Contains(deviceId))
            {
                ConnectedDeviceIds.Remove(deviceId);
            }
        }

        /// <summary>
        /// Device client sends a message to server.
        /// </summary>
        /// <param name="message">The message to send.</param>
        protected virtual void SendToServer(DevicesSyncMessage message, int? channelIdOrDefault = null)
        {
            if (LogFilter.logInfo)
            {
                UnityEngine.Debug.Log("Client: sending message: " + message);
            }

            int channelId = (channelIdOrDefault != null) ? (int)channelIdOrDefault : DefaultChannelId;
            message.SenderConnectionId = NetworkManager.client.connection.connectionId;
            NetworkManager.client.SendByChannel(message.MessageType, message, channelId);
        }

        protected void SendToAllClients(DevicesSyncMessage message, int? channelIdOrDefault = null)
        {
            if (LogFilter.logInfo)
            {
                UnityEngine.Debug.Log("Server: transfer message from device client " + message.SenderConnectionId 
                    + " to all " + "device clients: " + message);
            }

            int channelId = (channelIdOrDefault != null) ? (int)channelIdOrDefault : DefaultChannelId;
            NetworkServer.SendByChannelToAll(message.MessageType, message, channelId);
        }

        protected void SendToClient(int connectionId, DevicesSyncMessage message)
        {
            if (LogFilter.logInfo)
            {
                UnityEngine.Debug.Log("Server: transfer message from device client " + message.SenderConnectionId 
                    + " to device client " + connectionId + ": " + message);
            }
            NetworkServer.SendToClient(connectionId, message.MessageType, message);
        }

        /// <summary>
        /// Logs the error code.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        protected virtual void OnError(NetworkMessage netMessage)
        {
            var errorMessage = netMessage.ReadMessage<ErrorMessage>();
            UnityEngine.Debug.LogError("Error: " + errorMessage.errorCode);
        }
    }
}
