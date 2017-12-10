using UnityEngine;

namespace DevicesSyncUnity.Messages
{
    /// <summary>
    /// Copy of <see cref="Transform"/>, usable for network messages.
    /// </summary>
    public class TransformInfo
    {
        // Properties

        public bool HasMoved { get; protected set; }

        // Variables

        public Vector3 position;
        public Quaternion rotation;

        // Methods

        /// <summary>
        /// Copies the <paramref name="transform"/> to <see cref="position"/> and <see cref="rotation"/>, and sets
        /// <see cref="HasMoved"/> to true if these vectors are different than the previous ones.
        /// </summary>
        /// <param name="transform">The transform to copy.</param>
        /// <param name="movementThreshold">
        /// The minimum difference of magnitude between the position vectors or between the rotation vectors to set
        /// <see cref="HasMoved"/> to true.
        /// </param>
        public void Update(Transform transform, float movementThreshold)
        {
            HasMoved = (position == null) || !VectorEquals(position, transform.position, movementThreshold)
                || !VectorEquals(rotation.eulerAngles, transform.rotation.eulerAngles, movementThreshold);
            position = transform.position;
            rotation = transform.rotation;
        }

        /// <summary>
        /// Updates the <paramref name="transform"/> with <see cref="position"/> and <see cref="rotation"/>.
        /// </summary>
        /// <param name="transform">The transform to udpate.</param>
        public void Restore(Transform transform)
        {
            transform.position = position;
            transform.rotation = rotation;
        }

        /// <summary>
        /// Return if two vectors are equals.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <param name="precision">The precision of the equality in meters.</param>
        /// <returns>If the two vectors are equals or not.</returns>
        protected bool VectorEquals(Vector3 v1, Vector3 v2, float precision = 0.001f)
        {
            return (v1 - v2).sqrMagnitude < (precision * precision);
        }
    }
}
