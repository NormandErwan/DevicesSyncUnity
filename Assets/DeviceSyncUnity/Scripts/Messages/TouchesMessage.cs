using System.Collections.Generic;
using UnityEngine;

namespace DeviceSyncUnity.Messages
{
    /// <summary>
    /// Message that contains device touches information from latest frame, and touches average information from
    /// previous frames.
    /// </summary>
    public class TouchesMessage : DevicesSyncMessage
    {
        // Properties

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }

        // Variables

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public int senderConnectionId;

        /// <summary>
        /// Copy of the latest <see cref="Input.touches"/>.
        /// </summary>
        public TouchInfo[] touches;

        /// <summary>
        /// Copy of the latest <see cref="Input.touches"/> with some fields summed up over the previous frames
        /// (<see cref="Touch.deltaPosition"/>, <see cref="Touch.deltaTime"/>, <see cref="Touch.tapCount"/>) and other
        /// averaged (<see cref="Touch.pressure"/>, <see cref="Touch.radius"/>, <see cref="Touch.radiusVariance"/>).
        /// </summary>
        public TouchInfo[] touchesAverage;

        // Methods

        /// <summary>
        /// Copies the current <see cref="Input.touches"/> to <see cref="touches"/>.
        /// </summary>
        public void UpdateInfo()
        {
            touches = new TouchInfo[Input.touchCount];
            for (int i = 0; i < Input.touchCount; i++)
            {
                touches[i] = Input.touches[i];
            }
        }

        /// <summary>
        /// Sets <see cref="touchesAverage"/> with <see cref="touches"/> and touches from previous frames.
        /// </summary>
        /// <param name="previousTouchesStack">Touches from previous frames.</param>
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
