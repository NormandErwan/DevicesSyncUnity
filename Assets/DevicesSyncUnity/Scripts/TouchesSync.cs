using DevicesSyncUnity.Messages;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DevicesSyncUnity
{
    /// <summary>
    /// Synchronize touches information between devices with <see cref="AccelerationMessage"/>.
    /// </summary>
    public class TouchesSync : DevicesSyncInterval
    {
        // Editor fields

        [SerializeField]
        [Tooltip("The devices' static information to use.")]
        private DevicesInfoSync deviceInfoSync;

        // Properties

        /// <summary>
        /// Gets or sets the devices' static information to use.
        /// </summary>
        public DevicesInfoSync DeviceInfoSync { get { return deviceInfoSync; } set { deviceInfoSync = value; } }

        /// <summary>
        /// Gets the latest touches information from currently connected devices.
        /// </summary>
        public Dictionary<int, TouchesMessage> Touches { get; protected set; }

        // Events

        /// <summary>
        /// Called on server and on device client when a <see cref="TouchesMessage"/> is received.
        /// </summary>
        public event Action<TouchesMessage> TouchesReceived = delegate { };

        // Variables

        protected bool noTouchesLastMessage = false;
        private TouchesMessage touchesMessage = new TouchesMessage();
        protected bool initialAutoStartSending;

        // Methods

        /// <summary>
        /// Initializes the properties.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            initialAutoStartSending = AutoStartSending;
            AutoStartSending = false;

            Touches = new Dictionary<int, TouchesMessage>();
            MessageTypes.Add(touchesMessage.MessageType);
        }

        /// <summary>
        /// Subscribes to <see cref="TouchesSync.ClientDeviceInfoReceived"/>.
        /// </summary>
        protected override void Start()
        {
            base.Start();

            if (SyncMode != SyncMode.ReceiverOnly && initialAutoStartSending && isClient)
            {
                DeviceInfoSync.DeviceInfoReceived += DeviceInfoSync_ClientDeviceInfoReceived;
            }
        }

        /// <summary>
        /// Sends current and previous touches information if required and if there are touches information and
        /// there were touches information at the previous interval.
        /// </summary>
        /// <param name="sendToServerThisFrame">If the touches information should be sent this frame.</param>
        protected override void OnSendToServerIntervalIteration(bool sendToServerThisFrame)
        {
            touchesMessage.Update();

            if (sendToServerThisFrame)
            {
                bool noTouches = (touchesMessage.touches.Length == 0);
                if (!noTouches || !noTouchesLastMessage)
                {
                    SendToServer(touchesMessage);
                    touchesMessage.Reset();
                }
                noTouchesLastMessage = noTouches;
            }
        }

        /// <summary>
        /// Calls <see cref="ProcessMessageReceived(NetworkMessage)"/>.
        /// </summary>
        protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
        {
            return ProcessMessageReceived(netMessage);
        }

        /// <summary>
        /// Calls <see cref="ProcessMessageReceived(NetworkMessage)"/>.
        /// </summary>
        protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
        {
            return ProcessMessageReceived(netMessage);
        }

        /// <summary>
        /// Device client removes the disconnected device from <see cref="Touches"/>.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        protected override void OnClientDeviceDisconnectedReceived(DeviceDisconnectedMessage deviceDisconnectedMessage)
        {
            Touches.Remove(deviceDisconnectedMessage.SenderConnectionId);
        }

        /// <summary>
        /// Starts sending touches as the server has transmited current device's static information.
        /// </summary>
        protected virtual void DeviceInfoSync_ClientDeviceInfoReceived(DeviceInfoMessage message)
        {
            StartSending();
            DeviceInfoSync.DeviceInfoReceived -= DeviceInfoSync_ClientDeviceInfoReceived;
        }

        /// <summary>
        /// Device client updates <see cref="Touches"/> and calls <see cref="TouchesReceived"/>.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected TouchesMessage ProcessMessageReceived(NetworkMessage netMessage)
        {
            var touchesMessage = netMessage.ReadMessage<TouchesMessage>();
            Touches[touchesMessage.SenderConnectionId] = touchesMessage;
            TouchesReceived.Invoke(touchesMessage);
            return touchesMessage;
        }
    }
}
