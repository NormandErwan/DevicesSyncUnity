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
        /// Copy of main camera's <see cref="Camera.pixelHeight"/>.
        /// </summary>
        public int cameraPixelHeight;

        /// <summary>
        /// Copy of main camera's <see cref="Camera.pixelWidth"/>.
        /// </summary>
        public int cameraPixelWidth;

        // Methods

        /// <summary>
        /// Updates the public variables with device <see cref="Input"/> and <see cref="Camera.main"/> information.
        /// </summary>
        public void UpdateInfo()
        {
            multiTouchEnabled = Input.multiTouchEnabled;
            stylusTouchSupported = Input.stylusTouchSupported;
            touchPressureSupported = Input.touchPressureSupported;
            cameraPixelHeight = Camera.main.pixelHeight;
            cameraPixelWidth = Camera.main.pixelWidth;
        }
    }
}
