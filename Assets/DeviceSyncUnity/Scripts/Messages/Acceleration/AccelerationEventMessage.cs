using UnityEngine;

namespace DeviceSyncUnity.Messages
{
    public class AccelerationEventMessage
    {
        // Variables

        public Vector3 acceleration;
        public float deltaTime;

        // Methods

        public static implicit operator AccelerationEventMessage(AccelerationEvent accelerationEvent)
        {
            var accelerationEventMessage = new AccelerationEventMessage();
            accelerationEventMessage.acceleration = accelerationEvent.acceleration;
            accelerationEventMessage.deltaTime = accelerationEvent.deltaTime;
            return accelerationEventMessage;
        }
    }
}
