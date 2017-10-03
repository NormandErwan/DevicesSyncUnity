using DevicesSyncUnity.Messages;
using Lean.Touch;
using UnityEngine;

namespace DevicesSyncUnity.Examples.Messages
{
    /// <summary>
    /// Message that contains <see cref="LeanTouch"/>'s static information.
    /// </summary>
    public class LeanTouchInfoMessage : DevicesSyncMessage
    {
        // Properties

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }

        /// <summary>
        /// See <see cref="DevicesSyncMessage.MessageType"/>.
        /// </summary>
        public override short MessageType { get { return Messages.MessageType.LeanTouchInfo; } }

        /// <summary>
        /// Copy of <see cref="LeanTouch.CurrentGuiLayers"/>.
        /// </summary>
        public LayerMask CurrentGuiLayers { get { return currentGuiLayers; } }

        // Variables

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public int senderConnectionId;

        /// <summary>
        /// Copy of <see cref="LeanTouch.CurrentTapThreshold"/>.
        /// </summary>
        public float CurrentTapThreshold;

        /// <summary>
        /// Copy of <see cref="LeanTouch.CurrentSwipeThreshold"/>.
        /// </summary>
        public float CurrentSwipeThreshold;

        /// <summary>
        /// Copy of <see cref="LeanTouch.CurrentReferenceDpi"/>.
        /// </summary>
        public int CurrentReferenceDpi;

        /// <summary>
        /// Copy of <see cref="LeanTouch.CurrentGuiLayers"/>'s value.
        /// </summary>
        public int CurrentGuiLayersValue;

        /// <summary>
        /// Copy of <see cref="LeanTouch.ScalingFactor"/>.
        /// </summary>
        public float ScalingFactor;

        private LayerMask currentGuiLayers;

        // Methods

        /// <summary>
        /// Updates the public variables with current instance of <see cref="LeanTouch"/>.
        /// </summary>
        public void UpdateInfo()
        {
            CurrentTapThreshold = LeanTouch.CurrentTapThreshold;
            CurrentSwipeThreshold = LeanTouch.CurrentSwipeThreshold;
            CurrentReferenceDpi = LeanTouch.CurrentReferenceDpi;
            CurrentGuiLayersValue = LeanTouch.CurrentGuiLayers.value;
            ScalingFactor = LeanTouch.ScalingFactor;
        }

        public void RestoreInfo()
        {
            currentGuiLayers = new LayerMask()
            {
                value = CurrentGuiLayersValue
            };
        }
    }
}
