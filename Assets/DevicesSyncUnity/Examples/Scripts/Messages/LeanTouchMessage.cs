using DevicesSyncUnity.Messages;
using Lean.Touch;
using System;
using System.Collections.Generic;

namespace DevicesSyncUnity.Examples.Messages
{
    /// <summary>
    /// Message that contains device static <see cref="Input"/> and <see cref="Camera.main"/> information.
    /// </summary>
    public class LeanTouchMessage : DevicesSyncMessage
    {
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

        private Queue<LeanFingerInfo> fingers = new Queue<LeanFingerInfo>();
        private Queue<LeanFingerInfo> inactiveFingers = new Queue<LeanFingerInfo>();
        private bool capturingEvents = false;
        private Queue<LeanFingerInfo> fingersDown = new Queue<LeanFingerInfo>();

        // Methods

        public void SetCapturingEvents(bool value)
        {
            if (value && !capturingEvents)
            {
                capturingEvents = true;
                LeanTouch.OnFingerDown += LeanTouch_OnFingerDown;
            }
            else if (!value && capturingEvents)
            {
                capturingEvents = false;
                LeanTouch.OnFingerDown -= LeanTouch_OnFingerDown;
            }
        }

        public void UpdateInfo()
        {
            Fingers = GetFingersInfo(LeanTouch.Fingers, fingers);
            InactiveFingers = GetFingersInfo(LeanTouch.InactiveFingers, inactiveFingers);
            FingersDown = fingersDown.ToArray();
        }

        public void Reset()
        {
            fingers.Clear();
            inactiveFingers.Clear();
            fingersDown.Clear();
        }

        public void RestoreInfo(LeanTouchInfoMessage leanTouchInfo)
        {
            foreach (var finger in Fingers)
            {
                finger.RestoreInfo(leanTouchInfo);
            }
        }

        protected void LeanTouch_OnFingerDown(LeanFinger leanFinger)
        {
            fingersDown.Enqueue(leanFinger);
        }

        protected LeanFingerInfo[] GetFingersInfo(List<LeanFinger> leanFingers, Queue<LeanFingerInfo> fingersQueue)
        {
            foreach (var finger in leanFingers)
            {
                fingersQueue.Enqueue(finger);
            }
            return fingersQueue.ToArray();
        }
    }
}
