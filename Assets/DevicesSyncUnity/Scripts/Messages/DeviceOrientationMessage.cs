using System.Collections.Generic;
using UnityEngine;

namespace DevicesSyncUnity.Messages
{
    /// <summary>
    /// Message that contains device orientation from current and previous frames.
    /// </summary>
    public class DeviceOrientationMessage : DevicesSyncMessage
    {
        // Properties

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }

        /// <summary>
        /// See <see cref="DevicesSyncMessage.MessageType"/>.
        /// </summary>
        public override short MessageType { get { return Messages.MessageType.DeviceOrientation; } }

        /// <summary>
        /// List of the <see cref="Input.deviceOrientation"/> from the current and previous frames.
        /// </summary>
        public Queue<DeviceOrientation> deviceOrientations { get; protected set; }

        // Variables

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public int senderConnectionId;

        /// <summary>
        /// Values of <see cref="deviceOrientation"/> transmitted through network.
        /// </summary>
        public int[] deviceOrientationValues;

        private Queue<int> deviceOrientationsQueue = new Queue<int>();
        private int? latestDeviceOrientation = null;

        // Methods

        /// <summary>
        /// Enqueues the current device orientation if it's different from the previous frame's one.
        /// </summary>
        public void UpdateInfo()
        {
            int deviceOrientationValue = (int)Input.deviceOrientation;
            if (deviceOrientationValue != latestDeviceOrientation)
            {
                deviceOrientationsQueue.Enqueue(deviceOrientationValue);
                latestDeviceOrientation = deviceOrientationValue;
            }
            deviceOrientationValues = deviceOrientationsQueue.ToArray();
        }

        /// <summary>
        /// Resets the list of device orientations.
        /// </summary>
        public void Reset()
        {
            deviceOrientationsQueue.Clear();
        }

        /// <summary>
        /// Populate <see cref="deviceOrientations"/> from <see cref="deviceOrientationValues"/>.
        /// </summary>
        public void RestoreInfo()
        {
            deviceOrientations = new Queue<DeviceOrientation>(deviceOrientationValues.Length);
            for (int i = 0; i < deviceOrientationValues.Length; i++)
            {
                deviceOrientations.Enqueue((DeviceOrientation)deviceOrientationValues[i]);
            }
        }
    }
}
