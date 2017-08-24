using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DeviceSyncUnity.Messages
{
    public class TouchesMessage : MessageBase
    {
        // Variables

        public int connectionId;

        public bool multiTouchEnabled;
        public bool stylusTouchSupported;
        public bool touchPressureSupported;
        public TouchMessage[] touches;
        public TouchMessage[] touchesAverage;

        public int cameraPixelHeigth;
        public int cameraPixelWidth;

        // Methods

        public virtual void Populate(int connectionId, Camera camera)
        {
            multiTouchEnabled = Input.multiTouchEnabled;
            stylusTouchSupported = Input.stylusTouchSupported;
            touchPressureSupported = Input.touchPressureSupported;

            touches = new TouchMessage[Input.touchCount];
            for (int i = 0; i < Input.touchCount; i++)
            {
                touches[i] = new TouchMessage(Input.touches[i]);
            }

            cameraPixelHeigth = camera.pixelHeight;
            cameraPixelWidth = camera.pixelWidth;
        }

        public virtual void SetTouchesAverage(Stack<TouchMessage[]> previousTouchesStack)
        {
            var touchesAverage = new List<TouchMessage>();
            var touchesStackCount = previousTouchesStack.Count;

            // Initialize
            foreach (var touch in touches)
            {
                touchesAverage.Add(new TouchMessage(touch.GetTouch()));
            }

            // Sum up with touches from previous frames
            while (previousTouchesStack.Count > 0)
            {
                var previousTouches = previousTouchesStack.Pop();
                foreach (var previousTouch in previousTouches)
                {
                    bool newTouch = true;
                    foreach (var touchAverage in touchesAverage)
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
            if (touchesStackCount > 1)
            {
                foreach (var touchAverage in touchesAverage)
                {
                    touchAverage.pressure /= touchesStackCount;
                    touchAverage.radius /= touchesStackCount;
                    touchAverage.radiusVariance /= touchesStackCount;
                }
            }

            this.touchesAverage = touchesAverage.ToArray();
        }
    }
}
