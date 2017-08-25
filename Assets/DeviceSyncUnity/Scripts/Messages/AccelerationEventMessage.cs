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

        public static implicit operator AccelerationEvent(AccelerationEventMessage accelerationEventMessage)
        {
            var accelerationEvent = new AccelerationEvent();
            accelerationEvent.acceleration = accelerationEventMessage.acceleration;
            accelerationEvent.deltaTime = accelerationEventMessage.deltaTime;
            return accelerationEvent;
        }
    }
}
