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
        /// Gets the list of ids of currently connected devices to the server.
        /// </summary>
        public static List<int> ConnectedDeviceIds { get; protected set; }

        /// <summary>
        /// The network manager to use for receive messages from the server.
        /// </summary>
        public NetworkManager NetworkManager { get { return networkManager; } set { networkManager = value; } }

        /// <summary>
        /// Gets or sets the synchronization mode between device clients and the server.
        /// </summary>
        public SyncMode SyncMode { get { return syncMode; } set { syncMode = value; } }

        /// <summary>
        /// Gets the default channel to use for sending messages.
        /// </summary>
        protected virtual int DefaultChannelId { get { return Channels.DefaultUnreliable; } }

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

        private static bool initializedOtherDeviceSync = false;
        private DeviceConnectedMessage deviceConnectedMessage = new DeviceConnectedMessage();
        private DeviceDisconnectedMessage deviceDisconnectedMessage = new DeviceDisconnectedMessage();

        // Methods

        /// <summary>
        /// Initializes the properties.
        /// </summary>
        protected virtual void Awake()
        {
            ConnectedDeviceIds = new List<int>();
            MessageTypes = new List<short>();
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
            
            // Configure the server handlers
            if (isServer)
            {
                if (!initializedOtherDeviceSync)
                {
                    NetworkServer.RegisterHandler(deviceConnectedMessage.MessageType, ServerDeviceConnected);
                    NetworkServer.RegisterHandler(MsgType.Disconnect, ServerClientDisconnected);
                }

                foreach (var messageType in MessageTypes)
                {
                    NetworkServer.RegisterHandler(messageType, ServerMessageReceived);
                }
            }

            // Configure the client handlers and declare the device to the server
            var client = NetworkManager.client;
            if (client != null && isClient)
            {
                if (SyncMode != SyncMode.SenderOnly)
                {
                    if (!initializedOtherDeviceSync)
                    {
                        client.RegisterHandler(deviceConnectedMessage.MessageType, ClientDeviceConnectedReceived);
                        client.RegisterHandler(deviceDisconnectedMessage.MessageType, ClientDeviceDisconnectedReceived);
                    }
                    DeviceConnected += OnClientDeviceConnected;
                    DeviceDisconnected += OnClientDeviceDisconnected;

                    if (!initializedOtherDeviceSync)
                    {
                        SendToServer(deviceConnectedMessage);
                    }

                    foreach (var messageType in MessageTypes)
                    {
                        client.RegisterHandler(messageType, ClientMessageReceived);
                    }
                }

                if (!initializedOtherDeviceSync && LogFilter.logError)
                {
                    client.RegisterHandler(MsgType.Error, OnError);
                }
            }

            initializedOtherDeviceSync = true;
        }

        protected virtual void OnDestroy()
        {
            if (isClient)
            {
                DeviceConnected -= OnClientDeviceConnected;
                DeviceDisconnected -= OnClientDeviceDisconnected;
            }
        }

        /// <summary>
        /// Server sends a <see cref="DeviceConnectedMessage"/> message to all device clients to inform another device has connected.
        /// </summary>
        /// <param name="netMessage">The connection message from the connected device client.</param>
        protected void ServerDeviceConnected(NetworkMessage netMessage)
        {
            deviceConnectedMessage.SenderConnectionId = netMessage.conn.connectionId;
            if (LogFilter.logInfo)
            {
                UnityEngine.Debug.Log("Server: device client " + deviceConnectedMessage.SenderConnectionId + " has connected");
            }

            ConnectedDeviceIds.Add(deviceConnectedMessage.SenderConnectionId);
            ConnectedDeviceIds.Sort();

            DeviceConnected.Invoke(deviceConnectedMessage.SenderConnectionId);
            SendToAllClients(deviceConnectedMessage, Channels.DefaultReliable);
        }

        /// <summary>
        /// Device client receives message from server that another device has connected.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        protected static void ClientDeviceConnectedReceived(NetworkMessage netMessage)
        {
            var message = netMessage.ReadMessage<DeviceConnectedMessage>();
            if (LogFilter.logInfo)
            {
                UnityEngine.Debug.Log("Client: device client " + message.SenderConnectionId + " has connected");
            }

            if (!ConnectedDeviceIds.Contains(message.SenderConnectionId)) // If it's simultaneously client and server, the device has already been added
            {
                ConnectedDeviceIds.Add(message.SenderConnectionId);
                ConnectedDeviceIds.Sort();
            }
            DeviceConnected.Invoke(message.SenderConnectionId);
        }

        /// <summary>
        /// Device client process the connection of another device.
        /// </summary>
        /// <param name="deviceId">The id of the connected device.</param>
        protected abstract void OnClientDeviceConnected(int deviceId);

        /// <summary>
        /// Server sends a <see cref="DeviceDisconnectedMessage"/> message to all device clients to inform another device has disconnected.
        /// </summary>
        /// <param name="netMessage">The disconnection message from the disconnected device client.</param>
        protected void ServerClientDisconnected(NetworkMessage netMessage)
        {
            deviceDisconnectedMessage.SenderConnectionId = netMessage.conn.connectionId;
            if (LogFilter.logInfo)
            {
                UnityEngine.Debug.Log("Server: device client " + deviceDisconnectedMessage.SenderConnectionId + " has disconnected");
            }

            ConnectedDeviceIds.Remove(deviceDisconnectedMessage.SenderConnectionId);
            DeviceDisconnected.Invoke(deviceDisconnectedMessage.SenderConnectionId);
            SendToAllClients(deviceDisconnectedMessage, Channels.DefaultReliable);
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

            ConnectedDeviceIds.Remove(message.SenderConnectionId);
            DeviceDisconnected.Invoke(message.SenderConnectionId);
        }

        /// <summary>
        /// Device client process the disconnection of another device.
        /// </summary>
        /// <param name="deviceId">The id of the disconnected device.</param>
        protected abstract void OnClientDeviceDisconnected(int deviceId);

        /// <summary>
        /// Server broadcasts any received networking message of type <see cref="MessageTypes"/> to all device clients.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        protected virtual void ServerMessageReceived(NetworkMessage netMessage)
        {
            var message = OnServerMessageReceived(netMessage);
            if (message != null)
            {
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
        /// Device client receives a network message of type <see cref="MessageTypes"/> from the server.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        protected virtual void ClientMessageReceived(NetworkMessage netMessage)
        {
            var message = OnClientMessageReceived(netMessage);
            if (message != null)
            {
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
        /// Device client sends a message to server.
        /// </summary>
        /// <param name="message">The message to send.</param>
        protected virtual void SendToServer(DevicesSyncMessage message, int? channelIdOrDefault = null)
        {
            if (LogFilter.logInfo)
            {
                UnityEngine.Debug.Log("Client: sending message " + message);
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

        protected void SendToClient(int deviceId, DevicesSyncMessage message)
        {
            if (LogFilter.logInfo)
            {
                UnityEngine.Debug.Log("Server: transfer message from device client " + message.SenderConnectionId
                    + " to device client " + deviceId + ": " + message);
            }
            NetworkServer.SendToClient(deviceId, message.MessageType, message);
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
