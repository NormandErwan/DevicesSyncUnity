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

        // Methods

        public void UpdateInfo()
        {
            UpdateFingersInfo(ref Fingers, LeanTouch.Fingers);
            UpdateFingersInfo(ref InactiveFingers, LeanTouch.InactiveFingers);
        }

        public void RestoreInfo(LeanTouchInfoMessage leanTouchInfo)
        {
            foreach (var finger in Fingers)
            {
                finger.RestoreInfo(leanTouchInfo);
            }
        }

        protected void UpdateFingersInfo(ref LeanFingerInfo[] fingers, List<LeanFinger> leanFingers)
        {
            Array.Resize(ref fingers, leanFingers.Count);
            for (int i = 0; i < fingers.Length; i++)
            {
                fingers[i] = leanFingers[i];
            }
        }
    }
}
