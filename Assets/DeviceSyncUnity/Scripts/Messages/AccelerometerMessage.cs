using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeviceSyncUnity.Messages
{
    public class AccelerometerMessage : DevicesSyncMessage
    {
        // Variables

        public Vector3 acceleration;
        public float deltaTime;
        public AccelerationEventMessage[] accelerationEvents;

        // Methods

        public virtual void Update()
        {
            acceleration += Input.acceleration;
            deltaTime += Time.deltaTime;

            int previousLength = 0;
            if (accelerationEvents == null)
            {
                accelerationEvents = new AccelerationEventMessage[Input.accelerationEventCount];
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
                accelerationEvents[i + previousLength] = new AccelerationEventMessage(Input.GetAccelerationEvent(i));
                i++;
            }
        }
    }
}
