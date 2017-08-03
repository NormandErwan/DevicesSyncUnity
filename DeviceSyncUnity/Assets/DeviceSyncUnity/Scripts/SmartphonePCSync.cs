using Newtonsoft.Json.Linq;
using UnityEngine;

namespace DeviceSyncUnity
{
    public class SmartphonePCSync : MonoBehaviour
    {
        // Editor fields

        [SerializeField]
        private DeviceSyncClient deviceSyncClient;

        // Methods

        protected void Start()
        {
#if (!UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
            deviceSyncClient.ConnectionStarted += (sender, args) =>
            {
                deviceSyncClient.Proxy.Subscribe("ReceiveTouches").Data += ReceiveTouches;
            };
#endif
        }

        protected void Update()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            if (deviceSyncClient.Connection.IsStarted)
            {
                deviceSyncClient.Proxy.Invoke("CallMethodOtherClients", "ReceiveTouches", Input.touches);
            }
#endif
        }

        protected void ReceiveTouches(object[] data)
        {
            var jtoken = data[0] as JToken;
            var touches = jtoken.ToObject<Touch[]>();

            int fingerCount = 0;
            foreach (Touch touch in touches)
            {
                if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                {
                    fingerCount++;
                }
            }
            Logger.Log("User has " + fingerCount + " finger(s) touching the screen");
        }
    }
}
