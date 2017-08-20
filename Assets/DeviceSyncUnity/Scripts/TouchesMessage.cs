using UnityEngine;
using UnityEngine.Networking;

namespace DeviceSyncUnity
{
    public class TouchesMessage : MessageBase
    {
        // Properties

        public int connectionId;

        public bool multiTouchEnabled;
        public bool stylusTouchSupported;
        public bool touchPressureSupported;
        public TouchMessage[] touches;

        public int cameraPixelHeigth;
        public int cameraPixelWidth;

        // Methods

        public void PopulateFromInput()
        {
            multiTouchEnabled = Input.multiTouchEnabled;
            stylusTouchSupported = Input.stylusTouchSupported;
            touchPressureSupported = Input.touchPressureSupported;

            touches = new TouchMessage[Input.touchCount];
            for (int i = 0; i < Input.touchCount; i++)
            {
                touches[i] = new TouchMessage(Input.touches[i]);
            }
        }

        public void PopulateFromCamera(Camera camera)
        {
            cameraPixelHeigth = camera.pixelHeight;
            cameraPixelWidth = camera.pixelWidth;
        }
    }
}
