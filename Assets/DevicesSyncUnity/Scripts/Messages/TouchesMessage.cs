using System.Collections.Generic;
using UnityEngine;

namespace DevicesSyncUnity.Messages
{
    /// <summary>
    /// Message that contains device touches information from current frame, and touches average information from
    /// previous frames.
    /// </summary>
    public class TouchesMessage : DevicesSyncMessage
    {
        // Properties

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }

        /// <summary>
        /// See <see cref="DevicesSyncMessage.MessageType"/>.
        /// </summary>
        public override short MessageType { get { return Messages.MessageType.Touches; } }

        // Variables

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public int senderConnectionId;

        /// <summary>
        /// Copy of the latest <see cref="Input.touches"/>.
        /// </summary>
        public TouchInfo[] touches;

        private Queue<TouchInfo> touchesQueue = new Queue<TouchInfo>();

        // Methods

        /// <summary>
        /// Copies the current <see cref="Input.touches"/> to <see cref="touches"/>.
        /// </summary>
        public void Update()
        {
            foreach (var touch in Input.touches)
            {
                touchesQueue.Enqueue(touch);
            }
            touches = touchesQueue.ToArray();
        }

        /// <summary>
        /// Reset the touches array.
        /// </summary>
        public void Reset()
        {
            touches = null;
            touchesQueue.Clear();
        }

        /// <summary>
        /// Restores the touches informations.
        /// </summary>
        /// <param name="deviceInfo">The associated information from which the touches come from.</param>
        public void Restore(DeviceInfoMessage deviceInfo)
        {
            foreach (var touch in touches)
            {
                touch.Restore(deviceInfo);
            }
        }
    }
}
