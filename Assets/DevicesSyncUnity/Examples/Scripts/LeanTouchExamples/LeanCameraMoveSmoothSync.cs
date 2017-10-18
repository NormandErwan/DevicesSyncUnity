using DevicesSyncUnity.Examples.Messages;
using UnityEngine;

namespace DevicesSyncUnity.Examples.LeanTouchExamples
{
    /// <summary>
    /// <see cref="Lean.Touch.LeanCameraMoveSmooth"/> ported to DevicesSyncUnity.
    /// </summary>
    public class LeanCameraMoveSmoothSync : LeanCameraMoveSync
    {
        [Tooltip("How quickly the zoom reaches the target value")]
        public float Dampening = 10.0f;

        private Vector3 remainingDelta;

        protected override void LeanTouchSync_LeanTouchReceived(LeanTouchMessage leanTouch)
        {
            var oldPosition = transform.localPosition;
            base.LeanTouchSync_LeanTouchReceived(leanTouch);

            remainingDelta += transform.localPosition - oldPosition;
            var factor = Lean.Touch.LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);
            var newDelta = Vector3.Lerp(remainingDelta, Vector3.zero, factor);

            transform.localPosition = oldPosition + remainingDelta - newDelta;

            remainingDelta = newDelta;
        }
    }
}