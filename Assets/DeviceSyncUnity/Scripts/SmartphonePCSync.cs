using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace DeviceSyncUnity
{
    public enum SendMode
    {
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
        private SendMode sendTouchesMode = SendMode.TimeInterval;

        [SerializeField]
        private float sendTouchesTimeInterval = 0.1f;

        [SerializeField]
        private uint sendTouchesFramesInterval = 2;

        // Properties

        public NetworkManager NetworkManager { get { return networkManager; } set { TryStartSync(value); } }

        public SendMode SendTouchesMode { get { return sendTouchesMode; } set { sendTouchesMode = value; } }
        public float SendTouchesTimeInterval { get { return sendTouchesTimeInterval; } set { sendTouchesTimeInterval = value; } }
        public uint SendTouchesFramesInterval { get { return sendTouchesFramesInterval; } set { sendTouchesFramesInterval = value; } }

        public Dictionary<int, TouchesMessage> Touches { get; protected set; }

        // Events

        // TODO: add a visual debugger that display the touches info on screen
        public event Action<TouchesMessage> ServerTouchesReceived = delegate { };
        public event Action<TouchesMessage> TouchesReceived = delegate { };

        // Variables

        protected TouchesMessage touches = new TouchesMessage();
        protected Stack<TouchMessage[]> touchesStack = new Stack<TouchMessage[]>();
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

            touchesMessage = (touchesMessage != null) ? touchesMessage : GetTouchesMessage();

            Debug.Log("Send touches (count: " + touchesMessage.touches.Length + ")", LogFilter.Debug);
            NetworkManager.client.Send(MessageType.Touches, touchesMessage);
        }

        public virtual TouchesMessage GetTouchesMessage()
        {
            touches.Populate(NetworkManager.client.connection.connectionId, Camera.main);
            return touches;
        }

        public virtual IEnumerator SendTouchesWithInterval()
        {
            while (true)
            {
                if (Input.touchCount == 0 && !touchesLastFrames)
                {
                    yield return null;
                }
                touchesLastFrames = Input.touchCount != 0;

                bool sendTouches = false;
                if (SendTouchesMode == SendMode.FramesInterval)
                {
                    sendTouches = (touchesStack.Count >= SendTouchesFramesInterval);
                }
                else if (SendTouchesMode == SendMode.TimeInterval)
                {
                    if (Time.unscaledTime - sendTouchesTimer >= SendTouchesTimeInterval)
                    {
                        sendTouchesTimer = Time.unscaledTime;
                        sendTouches = true;
                    }
                }

                var touchesMessage = GetTouchesMessage();
                if (!sendTouches)
                {
                    touchesStack.Push(touchesMessage.touches);
                }
                else
                {
                    touchesMessage.SetTouchesAverage(touchesStack);
                    SendTouches(touchesMessage);
                }
                yield return null;
            }
        }

        protected virtual void Start()
        {
            Touches = new Dictionary<int, TouchesMessage>();
            TryStartSync(); // TODO: handle when client is disconnected from server
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

                    if (Input.touchSupported)
                    {
                        StartCoroutine("SendTouchesWithInterval");
                    }
                }
            }
        }

        protected virtual void SendToAllClientsTouches(NetworkMessage netMessage)
        {
            var touchesMessage = netMessage.ReadMessage<TouchesMessage>();

            Debug.Log("Send to all clients touches from " + touchesMessage.connectionId + " (count: " + touchesMessage.touches.Length + ")", LogFilter.Debug);

            ServerTouchesReceived.Invoke(touchesMessage);
            NetworkServer.SendToAll(MessageType.Touches, touchesMessage);
        }

        protected virtual void ReceiveTouches(NetworkMessage netMessage)
        {
            var touchesMessage = netMessage.ReadMessage<TouchesMessage>();

            Debug.Log("Received touches from " + touchesMessage.connectionId + " (count: " + touchesMessage.touches.Length + ")", LogFilter.Debug);

            Touches[touchesMessage.connectionId] = touchesMessage;
            TouchesReceived.Invoke(touchesMessage);
        }

        protected virtual void OnError(NetworkMessage netMessage)
        {
            var errorMessage = netMessage.ReadMessage<ErrorMessage>();
            UnityEngine.Debug.LogError(errorMessage.errorCode);
        }
    }
}
