using UnityEngine;

namespace DeviceSyncUnity.Utilities
{
    /// <summary>
    /// Extensions for <see cref="Transform"/>.
    /// </summary>
    public static class TransformExtensions
    {
        /// <summary>
        /// Reset the position to zero, the rotation to identiy and the scale to one.
        /// </summary>
        public static void Reset(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
    }
}
