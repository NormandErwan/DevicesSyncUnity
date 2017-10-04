using System.Collections.Generic;
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

        /// <summary>
        /// See <see cref="DevicesSyncMessage.MessageType"/>.
        /// </summary>
        public override short MessageType { get { return Messages.MessageType.AccelerationEvents; } }

        public virtual Queue<AccelerationEventInfo> AccelerationEvents { get; protected set; }

        // Variables

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public int senderConnectionId;

        /// <summary>
        /// List of the <see cref="AccelerationEvent"/> from the previous frames.
        /// </summary>
        public AccelerationEventInfo[] accelerationEvents;

        private float? latestAccelerationMagnitude = null;

        // Methods

        public AccelerationEventsMessage() : base()
        {
            AccelerationEvents = new Queue<AccelerationEventInfo>();
        }

        /// <summary>
        /// Enqueue the current accelerations events if its acceleration is different from the previous one.
        /// </summary>
        public void UpdateInfo()
        {
            for (int index = 0; index < Input.accelerationEventCount; index++)
            {
                var accEvent = Input.GetAccelerationEvent(index);
                if (accEvent.acceleration.sqrMagnitude != latestAccelerationMagnitude)
                {
                    AccelerationEvents.Enqueue(accEvent);
                    latestAccelerationMagnitude = accEvent.acceleration.sqrMagnitude;
                }
            }
        }

        /// <summary>
        /// Sets <see cref="accelerationEvents"/> with the acceleration events queue.
        /// </summary>
        public void PrepareSending()
        {
            accelerationEvents = AccelerationEvents.ToArray();
            AccelerationEvents.Clear();
        }
    }
}
