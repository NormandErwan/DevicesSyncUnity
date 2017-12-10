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

        // Methods

        /// <summary>
        /// Initializes the properties and susbcribes to events.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            Touches = new Dictionary<int, TouchesMessage>();

            DeviceDisconnected += DevicesInfoSync_DeviceDisconnected;

            MessageTypes.Add(touchesMessage.MessageType);
        }

        /// <summary>
        /// Unsubscribes to events.
        /// </summary>
        protected virtual void OnDestroy()
        {
            DeviceDisconnected -= DevicesInfoSync_DeviceDisconnected;
        }

        /// <summary>
        /// Sends current and previous touches information if required and if there are touches information and
        /// there were touches information at the previous interval.
        /// </summary>
        /// <param name="sendToServerThisFrame">If the touches information should be sent this frame.</param>
        protected override void OnSendToServerIntervalIteration(bool sendToServerThisFrame)
        {
            if (sendToServerThisFrame)
            {
                touchesMessage.Update();

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
        /// Server calls <see cref="ProcessTouchesMessageReceived(TouchesMessage)"/>.
        /// </summary>
        protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
        {
            var touchesMessage = netMessage.ReadMessage<TouchesMessage>();
            ProcessTouchesMessageReceived(touchesMessage);
            return touchesMessage;
        }

        /// <summary>
        /// Device client calls <see cref="ProcessTouchesMessageReceived(TouchesMessage)"/>.
        /// </summary>
        protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
        {
            var touchesMessage = netMessage.ReadMessage<TouchesMessage>();
            if (!isServer)
            {
                ProcessTouchesMessageReceived(touchesMessage);
            }
            return touchesMessage;
        }

        /// <summary>
        /// Removes the disconnected device from <see cref="Touches"/>.
        /// </summary>
        /// <param name="deviceId">The id of the disconnected device.</param>
        protected virtual void DevicesInfoSync_DeviceDisconnected(int deviceId)
        {
            Touches.Remove(deviceId);
        }

        /// <summary>
        /// Updates <see cref="Touches"/> and calls the <see cref="TouchesReceived"/> event.
        /// </summary>
        /// <param name="touchesMesssage">The message received to process.</param>
        protected virtual void ProcessTouchesMessageReceived(TouchesMessage touchesMesssage)
        {
            if (DeviceInfoSync.DevicesInfo.ContainsKey(touchesMessage.SenderConnectionId))
            {
                touchesMessage.Restore(DeviceInfoSync.DevicesInfo[touchesMessage.SenderConnectionId]);
            }
            Touches[touchesMessage.SenderConnectionId] = touchesMessage;
            TouchesReceived(touchesMessage);
        }
    }
}
