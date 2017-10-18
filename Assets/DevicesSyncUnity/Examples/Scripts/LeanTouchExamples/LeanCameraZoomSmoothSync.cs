using DevicesSyncUnity.Examples.Messages;
using UnityEngine;

namespace DevicesSyncUnity.Examples.LeanTouchExamples
{
    public class LeanCameraZoomSmoothSync : LeanCameraZoomSync
    {
        [Tooltip("How quickly the zoom reaches the target value")]
        public float Dampening = 10.0f;

        private float currentZoom;

        protected override void OnEnable()
        {
            base.OnEnable();
            currentZoom = Zoom;
        }

        protected override void LeanTouchSync_LeanTouchReceived(LeanTouchMessage leanTouch)
        {
            base.LeanTouchSync_LeanTouchReceived(leanTouch);
            var factor = Lean.Touch.LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);
            currentZoom = Mathf.Lerp(currentZoom, Zoom, factor);
            SetZoom(currentZoom);
        }
    }
}