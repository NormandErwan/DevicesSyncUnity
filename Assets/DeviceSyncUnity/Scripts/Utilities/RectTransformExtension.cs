using UnityEngine;

namespace DeviceSyncUnity.Utilities
{
    public static class RectTransformExtensions
    {
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
