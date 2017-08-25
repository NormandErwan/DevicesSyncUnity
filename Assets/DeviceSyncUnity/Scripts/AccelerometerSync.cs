using DeviceSyncUnity.Messages;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DeviceSyncUnity
{
    public class AccelerometerSync : DevicesSync
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

        public Dictionary<int, AccelerometerMessage> Accelerations { get; protected set; }

        protected override short MessageType { get { return Messages.MessageType.Accelerometer; } }

        // Variables

        AccelerometerMessage accelerometerMessage = new AccelerometerMessage();

        // Events

        public event Action<AccelerometerMessage> ServerAccelerationReceived = delegate { };
        public event Action<AccelerometerMessage> ClientAccelerationReceived = delegate { };

        // Methods

        protected virtual void Awake()
        {
            Accelerations = new Dictionary<int, AccelerometerMessage>();
        }

        protected override void OnSendToServerIntervalIteration(bool send)
        {
            accelerometerMessage.Update();
            if (send)
            {
                SendToServer(accelerometerMessage);
                accelerometerMessage = new AccelerometerMessage();
            }
        }

        protected override DevicesSyncMessage OnSendToAllClientsInternal(NetworkMessage netMessage)
        {
            var accelerometerMessage = netMessage.ReadMessage<AccelerometerMessage>();
            ServerAccelerationReceived.Invoke(accelerometerMessage);
            return accelerometerMessage;
        }

        protected override DevicesSyncMessage OnClientReceiveInternal(NetworkMessage netMessage)
        {
            var accelerometerMessage = netMessage.ReadMessage<AccelerometerMessage>();
            Accelerations[accelerometerMessage.senderConnectionId] = accelerometerMessage;
            ClientAccelerationReceived.Invoke(accelerometerMessage);
            return accelerometerMessage;
        }
    }
}
