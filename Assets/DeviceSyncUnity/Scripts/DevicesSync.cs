using DeviceSyncUnity.Messages;
using System.Collections;
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

    public enum SendingMode
    {
        TimeInterval,
        FramesInterval
    }

    public abstract class DevicesSync : NetworkBehaviour
    {
        // Editor fields

        [SerializeField]
        private SyncMode syncMode = SyncMode.SenderAndReceiver;

        // Properties

        public SyncMode SyncMode { get { return syncMode; } set { syncMode = value; } }

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
                    if (SyncMode != SyncMode.SenderOnly)
                    {
                        client.RegisterHandler(MessageType, ClientReceive);
                    }

                    if (SyncMode != SyncMode.ReceiverOnly)
                    {
                        StartCoroutine("SendToServerWithInterval");
                    }

                    Utilities.Debug.Execute(() => client.RegisterHandler(MsgType.Error, OnError), LogFilter.Error);
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

            NetworkManager.singleton.client.Send(MessageType, message);
        }

        protected virtual IEnumerator SendToServerWithInterval()
        {
            while (true)
            {
                bool sendToServerThisFrame = false;
                if (SendingMode == SendingMode.FramesInterval)
                {
                    sendingFrameCounter++;
                    if (sendingFrameCounter >= SendingFramesInterval)
                    {
                        sendingFrameCounter = 0;
                        sendToServerThisFrame = true;
                    }
                }
                else if (SendingMode == SendingMode.TimeInterval)
                {
                    if (Time.unscaledTime - sendingTimer >= SendingTimeInterval)
                    {
                        sendingTimer = Time.unscaledTime;
                        sendToServerThisFrame = true;
                    }
                }

                OnSendToServerIntervalIteration(sendToServerThisFrame);
                yield return null;
            }
        }

        protected abstract void OnSendToServerIntervalIteration(bool send);

        protected virtual void SendToAllClients(NetworkMessage netMessage)
        {
            var message = OnSendToAllClientsInternal(netMessage);
            Utilities.Debug.Log("Server: sending message (type: " + message.GetType() + ") to all clients from client " + message.SenderInfo.connectionId, LogFilter.Debug);
            NetworkServer.SendToAll(MessageType, message);
        }

        protected abstract DevicesSyncMessage OnSendToAllClientsInternal(NetworkMessage netMessage);

        protected virtual void ClientReceive(NetworkMessage netMessage)
        {
            var message = OnClientReceiveInternal(netMessage);
            Utilities.Debug.Log("Client: received message (type: " + message.GetType() + ") from client " + message.SenderInfo.connectionId, LogFilter.Debug);
        }

        protected abstract DevicesSyncMessage OnClientReceiveInternal(NetworkMessage netMessage);

        protected virtual void OnError(NetworkMessage netMessage)
        {
            var errorMessage = netMessage.ReadMessage<ErrorMessage>();
            Utilities.Debug.LogError("Error: " + errorMessage.errorCode);
        }
    }
}
