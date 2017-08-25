using UnityEngine;

namespace DeviceSyncUnity.Messages
{
    public class AccelerationEventMessage
    {
        // Constructor

        public AccelerationEventMessage()
        {
        }

        public AccelerationEventMessage(AccelerationEvent accelerationEvent)
        {
            acceleration = accelerationEvent.acceleration;
            deltaTime = accelerationEvent.deltaTime;
        }

        // Variables

        public Vector3 acceleration;
        public float deltaTime;
    }
}
