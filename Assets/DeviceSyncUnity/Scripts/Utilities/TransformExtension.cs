using UnityEngine;

namespace DeviceSyncUnity.Utilities
{
    public static class TransformExtensions
    {
        public static void Reset(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
	}
}
