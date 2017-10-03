using DevicesSyncUnity.Messages;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DevicesSyncUnity
{
    /// <summary>
    /// Synchronize touches information between devices with <see cref="AccelerationEventsMessage"/>.
    /// </summary>
    public class TouchesSync : DevicesSyncInterval
    {
        // Editor fields

        [SerializeField]
        [Tooltip("Interval mode to use to send regularly messages.")]
        private SendingMode sendingMode = SendingMode.FramesInterval;

        [SerializeField]
        [Tooltip("The number of frame to use between each message in FramesInterval mode.")]
        private float sendingTimeInterval = 0.1f;

        [SerializeField]
        [Tooltip("The time in seconds to use between each message in TimeInterval mode.")]
        private uint sendingFramesInterval = 1;

        [SerializeField]
        private DevicesInfoSync deviceInfoSync;

        // Properties

        /// <summary>
        /// See <see cref="DevicesSyncInterval.SendingMode"/>.
        /// </summary>
        public override SendingMode SendingMode { get { return sendingMode; } set { sendingMode = value; } }

        /// <summary>
        /// See <see cref="DevicesSyncInterval.SendingTimeInterval"/>.
        /// </summary>
        public override float SendingTimeInterval { get { return sendingTimeInterval; } set { sendingTimeInterval = value; } }

        /// <summary>
        /// See <see cref="DevicesSyncInterval.SendingFramesInterval"/>.
        /// </summary>
        public override uint SendingFramesInterval { get { return sendingFramesInterval; } set { sendingFramesInterval = value; } }

        /// <summary>
        /// Gets or sets the devices' information to use.
        /// </summary>
        public DevicesInfoSync DeviceInfoSync { get { return deviceInfoSync; } set { deviceInfoSync = value; } }

        /// <summary>
        /// Gets latest touches information from currently connected devices.
        /// </summary>
        public Dictionary<int, TouchesMessage> Touches { get; protected set; }

        // Events

        /// <summary>
        /// Called on server when a new <see cref="TouchesMessage"/> is received from device.
        /// </summary>
        public event Action<TouchesMessage> ServerTouchesReceived = delegate { };

        /// <summary>
        /// Called on device client when a new <see cref="TouchesMessage"/> is received from another device.
        /// </summary>
        public event Action<TouchesMessage> ClientTouchesReceived = delegate { };

        // Variables

        protected Stack<TouchInfo[]> previousTouches = new Stack<TouchInfo[]>();
        protected bool noTouchesLastMessage = false;
        private TouchesMessage touchesMessage = new TouchesMessage();

        // Methods

        /// <summary>
        /// Initializes the properties.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            MessageTypes.Add(touchesMessage.MessageType);
            Touches = new Dictionary<int, TouchesMessage>();
        }

        /// <summary>
        /// Sends current and previous touches information if required and if there are touches information and
        /// there were touches information at the previous interval.
        /// </summary>
        /// <param name="sendToServerThisFrame">If the touches information should be sent this frame.</param>
        protected override void OnSendToServerIntervalIteration(bool sendToServerThisFrame)
        {
            touchesMessage.UpdateInfo();

            if (!sendToServerThisFrame)
            {
                // Stack with previous frames touches
                if (touchesMessage.touches.Length > 0)
                {
                    previousTouches.Push(touchesMessage.touches);
                }
            }
            else
            {
                // Calculate touches average and send if necessary
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

        /// <summary>
        /// Server invokes <see cref="ServerTouchesReceived"/>. 
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected override DevicesSyncMessage OnServerReceived(NetworkMessage netMessage)
        {
            var touchesMessage = netMessage.ReadMessage<TouchesMessage>();
            Touches[touchesMessage.SenderConnectionId] = touchesMessage;
            ServerTouchesReceived.Invoke(touchesMessage);
            return touchesMessage;
        }

        /// <summary>
        /// Device client updates <see cref="Touches"/> and calls <see cref="ClientTouchesReceived"/>.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected override DevicesSyncMessage OnClientReceived(NetworkMessage netMessage)
        {
            var touchesMessage = netMessage.ReadMessage<TouchesMessage>();
            Touches[touchesMessage.SenderConnectionId] = touchesMessage;
            ClientTouchesReceived.Invoke(touchesMessage);
            return touchesMessage;
        }

        /// <summary>
        /// Device client removes the disconnected device from <see cref="Touches"/>.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        protected override void OnClientDeviceDisconnectedReceived(DeviceInfoMessage deviceInfoMessage)
        {
            Touches.Remove(deviceInfoMessage.SenderConnectionId);
        }
    }
}
