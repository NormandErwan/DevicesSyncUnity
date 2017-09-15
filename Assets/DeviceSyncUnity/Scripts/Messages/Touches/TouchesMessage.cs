using System.Collections.Generic;
using UnityEngine;

namespace DeviceSyncUnity.Messages
{
    public class TouchesMessage : DevicesSyncMessage
    {
        // Properties

        public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }

        // Variables

        public int senderConnectionId;
        public TouchInfo[] touches;
        public TouchInfo[] touchesAverage;

        // Methods

        public void UpdateInfo()
        {
            touches = new TouchInfo[Input.touchCount];
            for (int i = 0; i < Input.touchCount; i++)
            {
                touches[i] = Input.touches[i];
            }
        }

        public virtual void SetTouchesAverage(Stack<TouchInfo[]> previousTouchesStack)
        {
            var touchesAverage = new List<TouchInfo>();

            // Initialize
            foreach (var touch in touches)
            {
                Touch touchCopy = touch;
                touchesAverage.Add(touchCopy);
            }

            // Sum up with touches from previous frames
            foreach (var previousTouches in previousTouchesStack)
            {
                foreach (var previousTouch in previousTouches)
                {
                    bool newTouch = true;
                    foreach (var touchAverage in touchesAverage) // TODO: improve this O(n^3)
                    {
                        if (touchAverage.fingerId == previousTouch.fingerId)
                        {
                            touchAverage.deltaPosition += previousTouch.deltaPosition;
                            touchAverage.deltaTime += previousTouch.deltaTime;
                            touchAverage.tapCount = Mathf.Max(touchAverage.tapCount, previousTouch.tapCount);
                            touchAverage.pressure += previousTouch.pressure;
                            touchAverage.radius += previousTouch.radius;
                            touchAverage.radiusVariance = previousTouch.radiusVariance;

                            newTouch = false;
                            break;
                        }
                    }

                    if (newTouch)
                    {
                        touchesAverage.Add(previousTouch);
                    }
                }
            }

            // Calculate the average
            if (previousTouchesStack.Count > 0)
            {
                int touchesCount = previousTouchesStack.Count + 1;
                foreach (var touchAverage in touchesAverage)
                {
                    touchAverage.pressure /= touchesCount;
                    touchAverage.radius /= touchesCount;
                    touchAverage.radiusVariance /= touchesCount;
                }
            }

            this.touchesAverage = touchesAverage.ToArray();
        }
    }
}
