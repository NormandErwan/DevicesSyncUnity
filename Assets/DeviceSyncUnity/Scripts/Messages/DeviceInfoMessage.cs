using UnityEngine;

namespace DeviceSyncUnity.Messages
{
    public class DeviceInfoMessage : DevicesSyncMessage
    {
        // Properties

        public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }

        // Variables

        public int senderConnectionId;

        public bool multiTouchEnabled;
        public bool stylusTouchSupported;
        public bool touchPressureSupported;

        public int cameraPixelHeight;
        public int cameraPixelWidth;

        // Methods

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
