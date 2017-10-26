using UnityEngine;

namespace DevicesSyncUnity.Messages
{
    /// <summary>
    /// Copy of <see cref="Transform"/>, usable for network messages.
    /// </summary>
    public class TransformInfo
    {
        // Properties

        public bool ShouldBeSynchronized { get; protected set; }

        // Variables

        public Vector3 position;
        public Quaternion rotation;

        // Methods

        public void Update(Transform transform, float movementThreshold)
        {
            ShouldBeSynchronized = (position == null) || !VectorEquals(position, transform.position, movementThreshold)
                || !VectorEquals(rotation.eulerAngles, transform.rotation.eulerAngles, movementThreshold);
            position = transform.position;
            rotation = transform.rotation;
        }

        public void Restore(Transform transform)
        {
            transform.position = position;
            transform.rotation = rotation;
        }

        protected bool VectorEquals(Vector3 v1, Vector3 v2, float precision)
        {
            return (v1 - v2).sqrMagnitude < (precision * precision);
        }
    }
}
