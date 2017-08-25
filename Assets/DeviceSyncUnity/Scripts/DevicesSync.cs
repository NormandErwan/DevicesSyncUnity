using DeviceSyncUnity.Messages;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace DeviceSyncUnity
{
    public enum SendingMode
    {
        TimeInterval,
        FramesInterval
    }

    public abstract class DevicesSync : NetworkBehaviour
    {
        // Properties

        public abstract SendingMode SendingMode { get; set; }
        public abstract uint SendingFramesInterval { get; set; }
        public abstract float SendingTimeInterval { get; set; }

        protected abstract short MessageType { get; }

        // Variables

        protected uint sendingFrameCounter = 0;
        protected float sendingTimer = 0;

        // Methods

        protected virtual void Start()
        {
            TryStartSync(); // TODO: handle when client is disconnected from server
        }

        protected virtual void TryStartSync()
        {
            if (NetworkManager.singleton != null)
            {
                if (isServer)
                {
                    NetworkServer.RegisterHandler(MessageType, SendToAllClients);
                }

                var client = NetworkManager.singleton.client;
                if (client != null && isClient)
                {
                    client.RegisterHandler(MessageType, ClientReceive);
                    Utilities.Debug.Execute(() => client.RegisterHandler(MsgType.Error, OnError), LogFilter.Error);

                    StartCoroutine("SendToServerWithInterval");
                }
            }
        }

        protected virtual void SendToServer(DevicesSyncMessage message)
        {
            if (NetworkManager.singleton == null || !NetworkManager.singleton.IsClientConnected())
            {
                Utilities.Debug.LogError("Can't send message to server : no NetworkManager, or not connected as client.");
                return;
            }
            Utilities.Debug.Log("Client: sending message (type: " + message.GetType() + ")", LogFilter.Debug);

            message.senderConnectionId = NetworkManager.singleton.client.connection.connectionId;
            NetworkManager.singleton.client.Send(MessageType, message);
        }

        protected virtual IEnumerator SendToServerWithInterval()
        {
            while (true)
            {
                bool send = false;
                if (SendingMode == SendingMode.FramesInterval)
                {
                    sendingFrameCounter++;
                    if (sendingFrameCounter >= SendingFramesInterval)
                    {
                        sendingFrameCounter = 0;
                        send = true;
                    }
                }
                else if (SendingMode == SendingMode.TimeInterval)
                {
                    if (Time.unscaledTime - sendingTimer >= SendingTimeInterval)
                    {
                        sendingTimer = Time.unscaledTime;
                        send = true;
                    }
                }

                OnSendToServerIntervalIteration(send);
                yield return null;
            }
        }

        protected abstract void OnSendToServerIntervalIteration(bool send);

        protected virtual void SendToAllClients(NetworkMessage netMessage)
        {
            var message = OnSendToAllClientsInternal(netMessage);
            Utilities.Debug.Log("Server: sending message (type: " + message.GetType() + ") to all clients from client " + message.senderConnectionId, LogFilter.Debug);
            NetworkServer.SendToAll(MessageType, message);
        }

        protected abstract DevicesSyncMessage OnSendToAllClientsInternal(NetworkMessage netMessage);

        protected virtual void ClientReceive(NetworkMessage netMessage)
        {
            var message = OnClientReceiveInternal(netMessage);
            Utilities.Debug.Log("Client: received message (type: " + message.GetType() + ") from client " + message.senderConnectionId, LogFilter.Debug);
        }

        protected abstract DevicesSyncMessage OnClientReceiveInternal(NetworkMessage netMessage);

        protected virtual void OnError(NetworkMessage netMessage)
        {
            var errorMessage = netMessage.ReadMessage<ErrorMessage>();
            Utilities.Debug.LogError("Error: " + errorMessage.errorCode);
        }
    }
}
