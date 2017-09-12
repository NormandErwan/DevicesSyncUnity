using DeviceSyncUnity.Messages;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DeviceSyncUnity
{
    public class AccelerationSync : DevicesSync
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

        public Dictionary<int, AccelerationMessage> Accelerations { get; protected set; }

        protected override short MessageType { get { return Messages.MessageType.Acceleration; } }

        // Variables

        protected AccelerationMessage accelerationMessage = new AccelerationMessage();

        // Events

        public event Action<AccelerationMessage> ServerAccelerationReceived = delegate { };
        public event Action<AccelerationMessage> ClientAccelerationReceived = delegate { };

        // Methods

        protected virtual void Awake()
        {
            Accelerations = new Dictionary<int, AccelerationMessage>();
        }

        protected override void OnSendToServerIntervalIteration(bool sendToServerThisFrame)
        {
            accelerationMessage.UpdateInfo();
            if (sendToServerThisFrame)
            {
                SendToServer(accelerationMessage);
                accelerationMessage = new AccelerationMessage();
            }
        }

        protected override DevicesSyncMessage OnSendToAllClientsInternal(NetworkMessage netMessage)
        {
            var accelerationMessage = netMessage.ReadMessage<AccelerationMessage>();
            ServerAccelerationReceived.Invoke(accelerationMessage);
            return accelerationMessage;
        }

        protected override DevicesSyncMessage OnClientReceiveInternal(NetworkMessage netMessage)
        {
            var accelerationMessage = netMessage.ReadMessage<AccelerationMessage>();
            Accelerations[accelerationMessage.senderInfo.connectionId] = accelerationMessage;
            ClientAccelerationReceived.Invoke(accelerationMessage);
            return accelerationMessage;
        }
    }
}
