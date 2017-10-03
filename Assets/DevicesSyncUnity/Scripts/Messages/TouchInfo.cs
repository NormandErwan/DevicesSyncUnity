using UnityEngine;

namespace DevicesSyncUnity.Messages
{
    /// <summary>
    /// Copy of the <see cref="Touch"/> structure, usable for network messages.
    /// </summary>
    public class TouchInfo
    {
        // Variables

        public int fingerId;
        public Vector2 position;
        public Vector2 rawPosition;
        public Vector2 deltaPosition;
        public float deltaTime;
        public int tapCount;
        public TouchPhase phase;
        public float pressure;
        public float maximumPossiblePressure;
        public TouchType type;
        public float altitudeAngle;
        public float azimuthAngle;
        public float radius;
        public float radiusVariance;

        // Methods

        /// <summary>
        /// Returns a TouchInfo copied from a <see cref="Touch"/> struct.
        /// </summary>
        public static implicit operator TouchInfo(Touch touch)
        {
            TouchInfo touchMessage = new TouchInfo();
            touchMessage.fingerId = touch.fingerId;
            touchMessage.position = touch.position;
            touchMessage.rawPosition = touch.rawPosition;
            touchMessage.deltaPosition = touch.deltaPosition;
            touchMessage.deltaTime = touch.deltaTime;
            touchMessage.tapCount = touch.tapCount;
            touchMessage.phase = touch.phase;
            touchMessage.pressure = touch.pressure;
            touchMessage.maximumPossiblePressure = touch.maximumPossiblePressure;
            touchMessage.type = touch.type;
            touchMessage.altitudeAngle = touch.altitudeAngle;
            touchMessage.azimuthAngle = touch.azimuthAngle;
            touchMessage.radius = touch.radius;
            touchMessage.radiusVariance = touch.radiusVariance;
            return touchMessage;
        }

        /// <summary>
        /// Returns a <see cref="Touch"/> struct copied from a TouchInfo.
        /// </summary>
        public static implicit operator Touch(TouchInfo touchMessage)
        {
            Touch touch = new Touch();
            touch.fingerId = touchMessage.fingerId;
            touch.position = touchMessage.position;
            touch.rawPosition = touchMessage.rawPosition;
            touch.deltaPosition = touchMessage.deltaPosition;
            touch.deltaTime = touchMessage.deltaTime;
            touch.tapCount = touchMessage.tapCount;
            touch.phase = touchMessage.phase;
            touch.pressure = touchMessage.pressure;
            touch.maximumPossiblePressure = touchMessage.maximumPossiblePressure;
            touch.type = touchMessage.type;
            touch.altitudeAngle = touchMessage.altitudeAngle;
            touch.azimuthAngle = touchMessage.azimuthAngle;
            touch.radius = touchMessage.radius;
            touch.radiusVariance = touchMessage.radiusVariance;
            return touch;
        }
    }
}
