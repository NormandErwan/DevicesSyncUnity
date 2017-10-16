using DevicesSyncUnity.Examples.Messages;
using UnityEngine;

namespace DevicesSyncUnity.Examples.LeanTouchExamples
{
    /// <summary>
    /// <see cref="Lean.Touch.LeanCameraMove"/> ported to DevicesSyncUnity.
    /// </summary>
    public class LeanCameraMove : LeanTouchSyncSubscriber
    {
        [Tooltip("The camera the movement will be done relative to")]
        public Camera Camera;

        [Tooltip("Ignore fingers with StartedOverGui?")]
        public bool IgnoreGuiFingers = true;

        [Tooltip("Ignore fingers if the finger count doesn't match? (0 = any)")]
        public int RequiredFingerCount;

        [Tooltip("The distance from the camera the world drag delta will be calculated from (this only matters for perspective cameras)")]
        public float Distance = 1.0f;

        [Tooltip("The sensitivity of the movement, use -1 to invert")]
        public float Sensitivity = 1.0f;

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
            var worldDelta = LeanGesture.GetWorldDelta(fingers, Distance, Camera);
            transform.position -= worldDelta * Sensitivity;
        }
    }
}