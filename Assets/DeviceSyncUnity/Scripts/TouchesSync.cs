using DeviceSyncUnity.Messages;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DeviceSyncUnity
{
    public class TouchesSync : DevicesSync
    {
        // Editor fields

        [SerializeField]
        private SendingMode sendingMode = SendingMode.TimeInterval;

        [SerializeField]
        private float sendingTimeInterval = 0.1f;

        [SerializeField]
        private uint sendingFramesInterval = 2;

        // Properties

        public override SendingMode SendingMode { get { return sendingMode; } set { sendingMode = value; } }
        public override float SendingTimeInterval { get { return sendingTimeInterval; } set { sendingTimeInterval = value; } }
        public override uint SendingFramesInterval { get { return sendingFramesInterval; } set { sendingFramesInterval = value; } }

        public Dictionary<int, TouchesMessage> Touches { get; protected set; }

        protected override short MessageType { get { return Messages.MessageType.Touches; } }

        // Variables

        protected Stack<TouchInfo[]> previousTouches = new Stack<TouchInfo[]>();
        protected bool noTouchesLastMessage = false;

        // Events

        public event Action<TouchesMessage> ServerTouchesReceived = delegate { };
        public event Action<TouchesMessage> ClientTouchesReceived = delegate { };

        // Methods

        protected virtual void Awake()
        {
            Touches = new Dictionary<int, TouchesMessage>();
        }

        protected override void OnSendToServerIntervalIteration(bool sendTosServerThisFrame)
        {
            var touchesMessage = new TouchesMessage();
            touchesMessage.UpdateInfo();

            if (!sendTosServerThisFrame)
            {
                if (touchesMessage.touches.Length > 0)
                {
                    previousTouches.Push(touchesMessage.touches);
                }
            }
            else
            {
                touchesMessage.SetTouchesAverage(previousTouches);
                previousTouches.Clear();

                bool noTouches = (touchesMessage.touchesAverage.Length == 0);
                if (!noTouches || !noTouchesLastMessage)
                {
                    SendToServer(touchesMessage);
                }
                noTouchesLastMessage = noTouches;
            }
        }

        protected override DevicesSyncMessage OnSendToAllClients(NetworkMessage netMessage)
        {
            var touchesMessage = netMessage.ReadMessage<TouchesMessage>();
            ServerTouchesReceived.Invoke(touchesMessage);
            return touchesMessage;
        }

        protected override DevicesSyncMessage OnClientReceive(NetworkMessage netMessage)
        {
            var touchesMessage = netMessage.ReadMessage<TouchesMessage>();
            Touches[touchesMessage.senderInfo.connectionId] = touchesMessage;
            ClientTouchesReceived.Invoke(touchesMessage);
            return touchesMessage;
        }
    }
}
