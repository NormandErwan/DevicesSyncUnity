using UnityEngine;

namespace DeviceSyncUnity
{
    public struct TouchMessage
    {
        // Constructors

        public TouchMessage(Touch touch)
        {
            phase = touch.phase;
        }

        // Properties

        public TouchPhase phase { get; set; }

        // Methods

        public Touch GetTouch()
        {
            Touch touch = new Touch();
            touch.phase = phase;
            return touch;
        }
    }
}
