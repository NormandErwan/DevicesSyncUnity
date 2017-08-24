using UnityEngine;

namespace DeviceSyncUnity
{
    public class TouchMessage
    {
        // Constructor

        public TouchMessage()
        {
        }

        public TouchMessage(Touch touch)
        {
            fingerId = touch.fingerId;
            position = touch.position;
            rawPosition = touch.rawPosition;
            deltaPosition = touch.deltaPosition;
            deltaTime = touch.deltaTime;
            tapCount = touch.tapCount;
            phase = touch.phase;
            pressure = touch.pressure;
            maximumPossiblePressure = touch.maximumPossiblePressure;
            type = touch.type;
            altitudeAngle = touch.altitudeAngle;
            azimuthAngle = touch.azimuthAngle;
            radius = touch.radius;
            radiusVariance = touch.radiusVariance;
        }

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

        public virtual Touch GetTouch()
        {
            Touch touch = new Touch();
            touch.fingerId = fingerId;
            touch.position = position;
            touch.rawPosition = rawPosition;
            touch.deltaPosition = deltaPosition;
            touch.deltaTime = deltaTime;
            touch.tapCount = tapCount;
            touch.phase = phase;
            touch.pressure = pressure;
            touch.maximumPossiblePressure = maximumPossiblePressure;
            touch.type = type;
            touch.altitudeAngle = altitudeAngle;
            touch.azimuthAngle = azimuthAngle;
            touch.radius = radius;
            touch.radiusVariance = radiusVariance;
            return touch;
        }
    }
}
