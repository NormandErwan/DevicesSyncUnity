using System.Collections.Generic;
using UnityEngine;

namespace DevicesSyncUnity.Messages
{
    /// <summary>
    /// Message that contains device's acceleration and acceleration events from current and previous frame.
    /// </summary>
    public class AccelerationMessage : DevicesSyncMessage
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

        // Variables

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public int senderConnectionId;

        /// <summary>
        /// List of the <see cref="AccelerationEvent"/> from the current and previous frames.
        /// </summary>
        public AccelerationEventInfo[] accelerationEvents;

        /// <summary>
        /// Sum of <see cref="Input.acceleration"/> from the current and previous frames.
        /// </summary>
        public Vector3 acceleration = Vector3.zero;

        /// <summary>
        /// Sum of <see cref="Time.deltaTime"/> from the current and previous frames.
        /// </summary>
        public float TimeDeltaTime;

        private Queue<AccelerationEventInfo> accelerationEventsQueue = new Queue<AccelerationEventInfo>();
        private float? latestAccEventAccelerationMagnitude = null;

        // Methods

        /// <summary>
        /// Enqueues the current accelerations events if there are differents from the previous frame.
        /// </summary>
        public void UpdateInfo()
        {
            for (int index = 0; index < Input.accelerationEventCount; index++)
            {
                var accEvent = Input.GetAccelerationEvent(index);
                if (accEvent.acceleration.sqrMagnitude != latestAccEventAccelerationMagnitude)
                {
                    accelerationEventsQueue.Enqueue(accEvent);
                    latestAccEventAccelerationMagnitude = accEvent.acceleration.sqrMagnitude;
                }
            }
            accelerationEvents = accelerationEventsQueue.ToArray();

            acceleration += Input.acceleration;
            TimeDeltaTime += Time.deltaTime;
        }

        /// <summary>
        /// Resets the acceleration and acceleration events lists.
        /// </summary>
        public void Reset()
        {
            accelerationEventsQueue.Clear();
            acceleration = Vector3.zero;
            TimeDeltaTime = 0;
        }
    }
}
