using System.Collections.Generic;
using UnityEngine;

namespace DeviceSyncUnity.Messages
{
    public class TouchesMessage : DevicesSyncMessage
    {
        // Variables

        public bool multiTouchEnabled;
        public bool stylusTouchSupported;
        public bool touchPressureSupported;
        public TouchMessage[] touches;
        public TouchMessage[] touchesAverage;

        public int cameraPixelHeigth;
        public int cameraPixelWidth;

        // Methods

        public virtual void Populate(Camera camera)
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

            // Initialize
            foreach (var touch in touches)
            {
                touchesAverage.Add(new TouchMessage(touch.GetTouch()));
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
            if (previousTouchesStack.Count > 1)
            {
                foreach (var touchAverage in touchesAverage)
                {
                    touchAverage.pressure /= previousTouchesStack.Count;
                    touchAverage.radius /= previousTouchesStack.Count;
                    touchAverage.radiusVariance /= previousTouchesStack.Count;
                }
            }

            this.touchesAverage = touchesAverage.ToArray();
        }
    }
}
