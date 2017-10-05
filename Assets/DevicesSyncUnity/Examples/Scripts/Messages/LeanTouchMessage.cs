using DevicesSyncUnity.Messages;
using Lean.Touch;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevicesSyncUnity.Examples.Messages
{
    /// <summary>
    /// Message that contains device static <see cref="Input"/> and <see cref="Camera.main"/> information.
    /// </summary>
    public class LeanTouchMessage : DevicesSyncMessage
    {
        /// <summary>
        /// Utility class to capture <see cref="LeanTouch"/> events in queues.
        /// </summary>
        protected abstract class LeanTouchEventCapture<T> where T : class
        {
            public Action<T> CapturingAction { get; protected set; }
            public Queue<LeanFingerInfo> CapturedEventsQueue { get; protected set; }

            public LeanTouchEventCapture()
            {
                CapturedEventsQueue = new Queue<LeanFingerInfo>();
            }
        }

        protected class LeanTouchFingerEventCapture : LeanTouchEventCapture<LeanFinger>
        {
            public LeanTouchFingerEventCapture() : base()
            {
                CapturingAction = leanFinger => CapturedEventsQueue.Enqueue(leanFinger);
            }
        }

        protected class LeanTouchGestureEventCapture : LeanTouchEventCapture<List<LeanFinger>>
        {
            public LeanTouchGestureEventCapture() : base()
            {
                CapturingAction = leanFingers => leanFingers.ForEach(leanFinger => CapturedEventsQueue.Enqueue(leanFinger));
            }
        }

        // Properties

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }

        /// <summary>
        /// See <see cref="DevicesSyncMessage.MessageType"/>.
        /// </summary>
        public override short MessageType { get { return Messages.MessageType.LeanTouch; } }

        // Variables

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public int senderConnectionId;

        /// <summary>
        /// Copy of <see cref="LeanTouch.Fingers"/>.
        /// </summary>
        public LeanFingerInfo[] Fingers;

        /// <summary>
        /// Copy of <see cref="LeanTouch.InactiveFingers"/>.
        /// </summary>
        public LeanFingerInfo[] InactiveFingers;

        /// <summary>
        /// List of <see cref="LeanTouch.OnFingerDown"/> captured since the latest <see cref="Reset"/>.
        /// </summary>
        public LeanFingerInfo[] FingersDown;

        /// <summary>
        /// List of <see cref="LeanTouch.OnFingerSet"/> captured since the latest <see cref="Reset"/>.
        /// </summary>
        public LeanFingerInfo[] FingersSet;

        /// <summary>
        /// List of <see cref="LeanTouch.OnFingerUp"/> captured since the latest <see cref="Reset"/>.
        /// </summary>
        public LeanFingerInfo[] FingersUp;

        /// <summary>
        /// List of <see cref="LeanTouch.OnFingerTap"/> captured since the latest <see cref="Reset"/>.
        /// </summary>
        public LeanFingerInfo[] FingersTap;

        /// <summary>
        /// List of <see cref="LeanTouch.OnFingerSwipe"/> captured since the latest <see cref="Reset"/>.
        /// </summary>
        public LeanFingerInfo[] FingersSwipe;

        /// <summary>
        /// List of <see cref="LeanTouch.OnGesture"/> captured since the latest <see cref="Reset"/>.
        /// </summary>
        public LeanFingerInfo[] Gestures;

        private Queue<LeanFingerInfo> fingers = new Queue<LeanFingerInfo>();
        private Queue<LeanFingerInfo> inactiveFingers = new Queue<LeanFingerInfo>();
        private bool capturingEvents = false;
        private LeanTouchFingerEventCapture onFingerDown = new LeanTouchFingerEventCapture();
        private LeanTouchFingerEventCapture onFingerSet = new LeanTouchFingerEventCapture();
        private LeanTouchFingerEventCapture onFingerUp = new LeanTouchFingerEventCapture();
        private LeanTouchFingerEventCapture onFingerTap = new LeanTouchFingerEventCapture();
        private LeanTouchFingerEventCapture onFingerSwipe = new LeanTouchFingerEventCapture();
        private LeanTouchGestureEventCapture onGesture = new LeanTouchGestureEventCapture();

        // Methods

        /// <summary>
        /// Sets if capturing all <see cref="LeanTouch"/> events.
        /// </summary>
        public void SetCapturingEvents(bool value)
        {
            if (value && !capturingEvents)
            {
                capturingEvents = true;
                LeanTouch.OnFingerDown += onFingerDown.CapturingAction;
                LeanTouch.OnFingerSet += onFingerSet.CapturingAction;
                LeanTouch.OnFingerUp += onFingerUp.CapturingAction;
                LeanTouch.OnFingerTap += onFingerTap.CapturingAction;
                LeanTouch.OnFingerSwipe += onFingerSwipe.CapturingAction;
                LeanTouch.OnGesture += onGesture.CapturingAction;
            }
            else if (!value && capturingEvents)
            {
                capturingEvents = false;
                LeanTouch.OnFingerDown -= onFingerDown.CapturingAction;
                LeanTouch.OnFingerSet -= onFingerSet.CapturingAction;
                LeanTouch.OnFingerUp -= onFingerUp.CapturingAction;
                LeanTouch.OnFingerTap -= onFingerTap.CapturingAction;
                LeanTouch.OnFingerSwipe -= onFingerSwipe.CapturingAction;
                LeanTouch.OnGesture -= onGesture.CapturingAction;
            }
        }

        /// <summary>
        /// Sets public fields from <see cref="LeanTouch"/> current frame information and from captured events.
        /// </summary>
        public void Update()
        {
            LeanTouch.Fingers.ForEach(finger => fingers.Enqueue(finger));
            LeanTouch.InactiveFingers.ForEach(finger => inactiveFingers.Enqueue(finger));

            Fingers = fingers.ToArray();
            InactiveFingers = inactiveFingers.ToArray();

            FingersDown = onFingerDown.CapturedEventsQueue.ToArray();
            FingersSet = onFingerSet.CapturedEventsQueue.ToArray();
            FingersUp = onFingerUp.CapturedEventsQueue.ToArray();
            FingersTap = onFingerTap.CapturedEventsQueue.ToArray();
            FingersSwipe = onFingerSwipe.CapturedEventsQueue.ToArray();
            Gestures = onGesture.CapturedEventsQueue.ToArray();
        }

        /// <summary>
        /// Clears the public fields and the captured event lists.
        /// </summary>
        public void Reset()
        {
            fingers.Clear();
            inactiveFingers.Clear();
            onFingerDown.CapturedEventsQueue.Clear();
            onFingerSet.CapturedEventsQueue.Clear();
            onFingerUp.CapturedEventsQueue.Clear();
            onFingerTap.CapturedEventsQueue.Clear();
            onFingerSwipe.CapturedEventsQueue.Clear();
            onGesture.CapturedEventsQueue.Clear();
        }

        /// <summary>
        /// Restores all transmited <see cref="Fingers"/>.
        /// </summary>
        /// <param name="leanTouchInfo">The associated <see cref="LeanTouch"/>'s static information.</param>
        public void Restore(LeanTouchInfoMessage leanTouchInfo)
        {
            foreach (var finger in Fingers)
            {
                finger.Restore(leanTouchInfo);
            }
        }

        public override string ToString()
        {
            var fingers = Fingers.Select(finger => finger.Index.ToString()).ToArray();
            return "LeanTouchMessage (Fingers: [" + string.Join(", ", fingers) + "])";
        }
    }
}
