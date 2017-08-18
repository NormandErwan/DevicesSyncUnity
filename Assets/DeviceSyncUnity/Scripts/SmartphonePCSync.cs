using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace DeviceSyncUnity
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class SmartphonePCSync : NetworkBehaviour
    {
        // Editor fields

        [SerializeField]
        private NetworkManager networkManager;

        // TODO: choose between an time interval or a frame interval

        [SerializeField]
        private float sendTouchesInterval = 0.1f;

        // Properties

        public NetworkManager NetworkManager { get { return networkManager; } set { TryStartSync(value); } }

        public float SendTouchesInterval { get { return sendTouchesInterval; } set { sendTouchesInterval = value; } }

        // TODO: dictionary of each sender with latest received touches values

        // Events

        // TODO: add a visual debugger that display the touches info on screen
        public event Action<TouchesMessage> TouchesReceived = delegate { };

        // Variables

        protected bool touchesLastFrames = false;
        protected bool syncStarted = false;

        // Methods

        protected virtual void Start()
        {
            TryStartSync();
        }

        protected virtual void TryStartSync(NetworkManager newNetworkManager = null)
        {
            if (syncStarted)
            {
                NetworkManager.client.UnregisterHandler(MessageType.Touches);
                NetworkManager.client.UnregisterHandler(MsgType.Error);
                networkManager = newNetworkManager;
                syncStarted = false;
            }

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

                    if (LogFilter.currentLogLevel <= LogFilter.Error)
                    {
                        NetworkManager.client.RegisterHandler(MsgType.Error, OnError);
                    }

                    // TODO: add coroutine to etablish that there is a sync client-server-client communication before start the send touches

#if UNITY_ANDROID || UNITY_IOS
                    StartCoroutine("SendTouchesCoroutine");
#endif
                }
            }
        }

#if UNITY_ANDROID || UNITY_IOS
        protected virtual IEnumerator SendTouchesCoroutine()
        {
            while (NetworkManager != null && NetworkManager.client != null)
            {
                SendTouches();
                // TODO: stack values every frame and send the stack
                // TODO: send also an average of the stack values
                // TODO: send Input.touchPressureSupported, Camera.pixelWidth and Camera.pixelHeigth
                yield return new WaitForSeconds(SendTouchesInterval);
            }
        }

        protected void SendTouches()
        {
            if (Input.touchCount == 0 && !touchesLastFrames)
            {
                return;
            }

            var message = new TouchesMessage();
            message.touches = new TouchMessage[Input.touchCount];
            for (int i = 0; i < message.touches.Length; i++)
            {
                message.touches[i] = new TouchMessage(Input.touches[i]);
            }

            touchesLastFrames = message.touches.Length != 0;

            if (LogFilter.currentLogLevel <= LogFilter.Debug)
            {
                Debug.Log("Send touches (count: " + message.touches.Length + ")");
            }

            NetworkManager.client.Send(MessageType.Touches, message);
        }
#endif

        protected virtual void SendToAllClientsTouches(NetworkMessage netMessage)
        {
            var message = netMessage.ReadMessage<TouchesMessage>();
            if (LogFilter.currentLogLevel <= LogFilter.Debug)
            {
                Debug.Log("Send to all clients touches (count: " + message.touches.Length + ")");
            }
            NetworkServer.SendToAll(MessageType.Touches, message);
        }

        protected virtual void ReceiveTouches(NetworkMessage netMessage)
        {
            var message = netMessage.ReadMessage<TouchesMessage>();
            if (LogFilter.currentLogLevel <= LogFilter.Debug)
            {
                Debug.Log("Received touches (count: " + message.touches.Length + ")");
            }
            TouchesReceived.Invoke(message);
        }

        protected virtual void OnError(NetworkMessage netMessage)
        {
            var errorMessage = netMessage.ReadMessage<ErrorMessage>();
            Debug.LogError(errorMessage.errorCode);
        }
    }
}
