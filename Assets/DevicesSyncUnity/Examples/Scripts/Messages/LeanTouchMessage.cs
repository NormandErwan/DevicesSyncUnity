﻿using DevicesSyncUnity.Messages;
using Lean.Touch;
using System;

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

        public LeanFingerInfo[] Fingers;

        // Methods

        public void UpdateInfo()
        {
            Array.Resize(ref Fingers, LeanTouch.Fingers.Count);
            for (int i = 0; i < Fingers.Length; i++)
            {
                Fingers[i] = LeanTouch.Fingers[i];
            }
        }

        public void RestoreInfo(LeanTouchInfoMessage leanTouchInfo)
        {
            foreach (var finger in Fingers)
            {
                finger.RestoreInfo(leanTouchInfo);
            }
        }
    }
}
