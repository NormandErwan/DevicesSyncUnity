using UnityEngine;

namespace DeviceSyncUnity
{
    public struct TouchMessage
    {
        // Constructor

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

        // Properties

        public int fingerId { get; set; }
        public Vector2 position { get; set; }
        public Vector2 rawPosition { get; set; }
        public Vector2 deltaPosition { get; set; }
        public float deltaTime { get; set; }
        public int tapCount { get; set; }
        public TouchPhase phase { get; set; }
        public float pressure { get; set; }
        public float maximumPossiblePressure { get; set; }
        public TouchType type { get; set; }
        public float altitudeAngle { get; set; }
        public float azimuthAngle { get; set; }
        public float radius { get; set; }
        public float radiusVariance { get; set; }

        // Methods

        public Touch GetTouch()
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
