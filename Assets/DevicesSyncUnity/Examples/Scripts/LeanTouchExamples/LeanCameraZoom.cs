using DevicesSyncUnity.Examples.Messages;
using UnityEngine;

namespace DevicesSyncUnity.Examples.LeanTouchExamples
{
    /// <summary>
    /// <see cref="Lean.Touch.LeanCameraZoom"/> ported to DevicesSyncUnity.
    /// </summary>
    [ExecuteInEditMode]
    public class LeanCameraZoom : LeanTouchSyncSubscriber
    {
        [Tooltip("The camera that will be zoomed")]
        public Camera Camera;

        [Tooltip("Ignore fingers with StartedOverGui?")]
        public bool IgnoreGuiFingers = true;

        [Tooltip("Allows you to force rotation with a specific amount of fingers (0 = any)")]
        public int RequiredFingerCount;

        [Tooltip("If you want the mouse wheel to simulate pinching then set the strength of it here")]
        [Range(-1.0f, 1.0f)]
        public float WheelSensitivity;

        [Tooltip("The current FOV/Size")]
        public float Zoom = 50.0f;

        [Tooltip("Limit the FOV/Size?")]
        public bool ZoomClamp;

        [Tooltip("The minimum FOV/Size we want to zoom to")]
        public float ZoomMin = 10.0f;

        [Tooltip("The maximum FOV/Size we want to zoom to")]
        public float ZoomMax = 60.0f;

        protected virtual void OnEnable()
        {
            LeanTouchSync.LeanTouchReceived += LeanTouchSync_LeanTouchReceived;
        }

        protected virtual void OnDisable()
        {
            LeanTouchSync.LeanTouchReceived -= LeanTouchSync_LeanTouchReceived;
        }

        protected virtual void LeanTouchSync_LeanTouchReceived(LeanTouchMessage leanTouch)
        {
            var fingers = leanTouch.GetFingers(IgnoreGuiFingers, RequiredFingerCount);
            var pinchRatio = LeanGesture.GetPinchRatio(fingers, WheelSensitivity);
            Zoom *= pinchRatio;
            if (ZoomClamp == true)
            {
                Zoom = Mathf.Clamp(Zoom, ZoomMin, ZoomMax);
            }
            SetZoom(Zoom);
        }

        protected void SetZoom(float current)
        {
            if (Camera.orthographic == true)
            {
                Camera.orthographicSize = current;
            }
            else
            {
                Camera.fieldOfView = current;
            }
        }
    }
}