using UnityEngine;

namespace DevicesSyncUnity.Messages
{
    /// <summary>
    /// Copy of the <see cref="AccelerationEvent"/> structure, usable for network messages.
    /// </summary>
    public class AccelerationEventInfo
    {
        // Variables

        public Vector3 acceleration;
        public float deltaTime;

        // Methods

        /// <summary>
        /// Returns a AccelerationEventInfo copied from a <see cref="AccelerationEvent"/> struct.
        /// </summary>
        public static implicit operator AccelerationEventInfo(AccelerationEvent accelerationEvent)
        {
            var accelerationEventMessage = new AccelerationEventInfo();
            accelerationEventMessage.acceleration = accelerationEvent.acceleration;
            accelerationEventMessage.deltaTime = accelerationEvent.deltaTime;
            return accelerationEventMessage;
        }
    }
}
