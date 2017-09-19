using UnityEngine;

namespace DeviceSyncUnity.Utilities
{
    /// <summary>
    /// Extensions for <see cref="Transform"/>.
    /// </summary>
    public static class RectTransformExtensions
    {
        /// <summary>
        /// Stretchs to the fit the size of its parent.
        /// </summary>
        public static void Stretch(this RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector3.zero;
            rectTransform.pivot = 0.5f * Vector3.one;
        }
    }
}
