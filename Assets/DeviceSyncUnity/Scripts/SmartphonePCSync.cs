using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace DeviceSyncUnity
{
    public enum SendTouchesMode
    {
        NoSending,
        TimeInterval,
        FramesInterval
    }

    [RequireComponent(typeof(NetworkIdentity))]
    public class SmartphonePCSync : NetworkBehaviour
    {
        // Editor fields

        [SerializeField]
        private NetworkManager networkManager;

        [SerializeField]
        private SendTouchesMode sendTouchesMode = SendTouchesMode.TimeInterval;

        [SerializeField]
        private float sendTouchesTimeInterval = 0.1f;

        [SerializeField]
        private uint sendTouchesFramesInterval = 1;

        // Properties

        public NetworkManager NetworkManager { get { return networkManager; } set { TryStartSync(value); } }

        public SendTouchesMode SendTouchesMode { get { return sendTouchesMode; } set { sendTouchesMode = value; } }
        public float SendTouchesTimeInterval { get { return sendTouchesTimeInterval; } set { sendTouchesTimeInterval = value; } }
        public uint SendTouchesFramesInterval { get { return sendTouchesFramesInterval; } set { sendTouchesFramesInterval = value; } }

        public Dictionary<int, TouchesMessage> Touches { get; protected set; }

        // Events

        // TODO: add a visual debugger that display the touches info on screen
        public event Action<TouchesMessage> ServerTouchesReceived = delegate { };
        public event Action<TouchesMessage> TouchesReceived = delegate { };

        // Variables

        protected uint sendTouchesFramesCount = 0;
        protected float sendTouchesTimer = 0;
        protected bool touchesLastFrames = false;
        protected bool syncStarted = false;

        // Methods

        public virtual void SendTouches(TouchesMessage touchesMessage = null)
        {
            if (NetworkManager == null || NetworkManager.client == null || !isClient)
            {
                Debug.LogError("Can't send touches : no NetworkManager, or not connected as client.");
                return;
            }

            if (touchesMessage == null)
            {
                touchesMessage = GetTouchesMessage();
            }
            Debug.Log("Send touches (count: " + touchesMessage.touches.Length + ")", LogFilter.Debug);
            NetworkManager.client.Send(MessageType.Touches, touchesMessage);
        }

        public virtual TouchesMessage GetTouchesMessage()
        {
            var message = new TouchesMessage();
            message.connectionId = NetworkManager.client.connection.connectionId;
            message.PopulateFromInput();
            message.PopulateFromCamera(Camera.main);
            return message;
        }

        public virtual IEnumerator SendTouchesWithInterval()
        {
            while (Input.touchSupported && SendTouchesMode != SendTouchesMode.NoSending)
            {
                if (Input.touchCount == 0 && !touchesLastFrames)
                {
                    yield return null;
                }
                touchesLastFrames = Input.touchCount != 0;

                var message = GetTouchesMessage();
                if (SendTouchesMode == SendTouchesMode.FramesInterval)
                {
                    sendTouchesFramesCount++;
                    if (sendTouchesFramesCount >= SendTouchesFramesInterval)
                    {
                        sendTouchesFramesCount = 0;
                        SendTouches(message);
                    }
                }
                else if (SendTouchesMode == SendTouchesMode.TimeInterval)
                {
                    if (Time.unscaledTime - sendTouchesTimer >= SendTouchesTimeInterval)
                    {
                        sendTouchesTimer = Time.unscaledTime;
                        SendTouches(message);
                    }
                }

                // TODO: stack values
                // TODO: send also an average of the stack values
                yield return null;
            }
        }

        // TODO: handle client reconnection
        protected virtual void Start()
        {
            Touches = new Dictionary<int, TouchesMessage>();
            TryStartSync();
        }

        protected virtual void TryStartSync(NetworkManager newNetworkManager = null)
        {
            // Unsubscribe from the previous NetworkManager
            if (syncStarted)
            {
                StopCoroutine("SendTouchesWithInterval");

                NetworkManager.client.UnregisterHandler(MessageType.Touches);
                NetworkManager.client.UnregisterHandler(MsgType.Error);
                networkManager = newNetworkManager;
                syncStarted = false;
            }

            // Subscribe to the NetworkServer and the NetworkManager's client
            if (NetworkManager != null)
            {
                syncStarted = true;

                if (isServer)
                {
                    NetworkServer.RegisterHandler(MessageType.Touches, SendToAllClientsTouches);
                }

                if (NetworkManager.client != null && isClient)
                {
                    NetworkManager.client.RegisterHandler(MessageType.Touches, ReceiveTouches);
                    Debug.Execute(() => NetworkManager.client.RegisterHandler(MsgType.Error, OnError), LogFilter.Error);
                    StartCoroutine("SendTouchesWithInterval");
                }
            }
        }

        protected virtual void SendToAllClientsTouches(NetworkMessage netMessage)
        {
            var message = netMessage.ReadMessage<TouchesMessage>();

            Debug.Log("Send to all clients touches from " + message.connectionId + " (count: " + message.touches.Length + ")", LogFilter.Debug);

            ServerTouchesReceived.Invoke(message);
            NetworkServer.SendToAll(MessageType.Touches, message);
        }

        protected virtual void ReceiveTouches(NetworkMessage netMessage)
        {
            var message = netMessage.ReadMessage<TouchesMessage>();

            Debug.Log("Received touches from " + message.connectionId + " (count: " + message.touches.Length + ")", LogFilter.Debug);

            Touches[message.connectionId] = message;
            TouchesReceived.Invoke(message);
        }

        protected virtual void OnError(NetworkMessage netMessage)
        {
            var errorMessage = netMessage.ReadMessage<ErrorMessage>();
            UnityEngine.Debug.LogError(errorMessage.errorCode);
        }
    }
}
