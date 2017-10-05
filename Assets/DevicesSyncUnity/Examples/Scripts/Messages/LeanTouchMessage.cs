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
        /// See <see cref="LeanTouch.Fingers"/>
        /// </summary>
        public LeanFingerInfo[] Fingers;

        /// <summary>
        /// See <see cref="LeanTouch.InactiveFingers"/>
        /// </summary>
        public LeanFingerInfo[] InactiveFingers;

        private Queue<LeanFingerInfo> fingersQueue = new Queue<LeanFingerInfo>();
        private Queue<LeanFingerInfo> inactiveFingersQueue = new Queue<LeanFingerInfo>();

        // Methods

        public void UpdateInfo()
        {
            Fingers = GetFingersInfo(LeanTouch.Fingers, fingersQueue);
            InactiveFingers = GetFingersInfo(LeanTouch.InactiveFingers, inactiveFingersQueue);
        }

        public void Reset()
        {
            fingersQueue.Clear();
            inactiveFingersQueue.Clear();
        }

        public void RestoreInfo(LeanTouchInfoMessage leanTouchInfo)
        {
            foreach (var finger in Fingers)
            {
                finger.RestoreInfo(leanTouchInfo);
            }
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
