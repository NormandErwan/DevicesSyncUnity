using UnityEngine;

namespace DeviceSyncUnity.Messages
{
    public class AccelerometerMessage : DevicesSyncMessage
    {
        // Variables

        public Vector3 acceleration = Vector3.zero;
        public float deltaTime = 0f;

        // Methods

        public virtual void Update()
        {
            acceleration += Input.acceleration;
            deltaTime += Time.deltaTime;
        }
    }
}
