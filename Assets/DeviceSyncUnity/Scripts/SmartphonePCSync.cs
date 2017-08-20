﻿using System;
using System.Collections;
using System.Collections.Generic;
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

        public Dictionary<int, TouchesMessage> Touches { get; protected set; }

        // Events

        // TODO: add a visual debugger that display the touches info on screen
        public event Action<TouchesMessage> ServerTouchesReceived = delegate { };
        public event Action<TouchesMessage> TouchesReceived = delegate { };

        // Variables

        protected bool touchesLastFrames = false;
        protected bool syncStarted = false;

        // Methods

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
            message.connectionId = NetworkManager.client.connection.connectionId;
            message.PopulateFromInput();
            message.PopulateFromCamera(Camera.main);

            Debug.Log("Send touches (count: " + message.touches.Length + ")", LogFilter.Debug);

            touchesLastFrames = message.touches.Length != 0;
            NetworkManager.client.Send(MessageType.Touches, message);
        }
#endif

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
