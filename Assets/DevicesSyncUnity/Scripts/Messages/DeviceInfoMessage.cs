using UnityEngine;

namespace DevicesSyncUnity.Messages
{
    /// <summary>
    /// Message that contains device static <see cref="Input"/> and <see cref="Camera.main"/> information.
    /// </summary>
    public class DeviceInfoMessage : DevicesSyncMessage
    {
        // Properties

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }

        /// <summary>
        /// See <see cref="DevicesSyncMessage.MessageType"/>.
        /// </summary>
        public override short MessageType { get { return Messages.MessageType.DeviceInfo; } }

        // Variables

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public int senderConnectionId;

        /// <summary>
        /// Copy of <see cref="Input.multiTouchEnabled"/>.
        /// </summary>
        public bool multiTouchEnabled;

        /// <summary>
        /// Copy of <see cref="Input.stylusTouchSupported"/>.
        /// </summary>
        public bool stylusTouchSupported;

        /// <summary>
        /// Copy of <see cref="Input.touchPressureSupported"/>.
        /// </summary>
        public bool touchPressureSupported;

        /// <summary>
        /// Copy of main camera's <see cref="Screen.height"/>.
        /// </summary>
        public int screenHeight;

        /// <summary>
        /// Copy of main camera's <see cref="Screen.width"/>.
        /// </summary>
        public int screenWidth;

        // Methods

        /// <summary>
        /// Updates the public variables with device <see cref="Input"/> and <see cref="Screen"/> information.
        /// </summary>
        public void UpdateInfo()
        {
            multiTouchEnabled = Input.multiTouchEnabled;
            stylusTouchSupported = Input.stylusTouchSupported;
            touchPressureSupported = Input.touchPressureSupported;
            screenHeight = Screen.height;
            screenWidth = Screen.width;
        }
    }
}
