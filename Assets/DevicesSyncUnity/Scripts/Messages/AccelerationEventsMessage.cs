using System;
using UnityEngine;

namespace DevicesSyncUnity.Messages
{
    /// <summary>
    /// Message that contains device static <see cref="Input"/> and <see cref="Camera.main"/> information.
    /// </summary>
    public class AccelerationEventsMessage : DevicesSyncMessage
    {
        // Properties

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }

        // Variables

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public int senderConnectionId;

        /// <summary>
        /// List of the <see cref="AccelerationEvent"/> from the previous frames.
        /// </summary>
        public AccelerationEventInfo[] accelerationEvents;

        // Methods

        /// <summary>
        /// Adds the current accelerations events to <see cref="accelerationEvents"/>.
        /// </summary>
        public void UpdateInfo()
        {
            int previousLength = 0;
            if (accelerationEvents == null)
            {
                accelerationEvents = new AccelerationEventInfo[Input.accelerationEventCount];
            }
            else
            {
                previousLength = accelerationEvents.Length;
                Array.Resize(ref accelerationEvents, accelerationEvents.Length + Input.accelerationEventCount);
            }

            int i = 0;
            while (i < Input.accelerationEventCount)
            {
                // TODO: check if the order of stacked events if correct
                accelerationEvents[i + previousLength] = Input.GetAccelerationEvent(i);
                i++;
            }
        }
    }
}
