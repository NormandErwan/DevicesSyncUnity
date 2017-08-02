using Newtonsoft.Json.Linq;
using UnityEngine;

namespace DeviceSyncUnity
{
    public class SmartphonePCSync : MonoBehaviour
    {
        [SerializeField]
        private DeviceSyncClient deviceSyncClient;

        protected void Start()
        {
#if (!UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
            deviceSyncClient.Proxy.Subscribe("ReceiveTouches").Data += data =>
            {
                Logger.Log("ReceiveTouches received");

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
            };
#endif
        }

        protected void Update()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            if (deviceSyncClient.Connection.IsStarted)
            {
                Touch[] touches = Input.touches;
                deviceSyncClient.Proxy.Invoke("CallMethodAllClients", "ReceiveTouches", touches).Finished += (rtsender, rte) =>
                {
                    Logger.Log("ReceiveTouches sent");
                };
            }
#endif
        }
    }
}
