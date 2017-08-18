using UnityEngine.Networking;

namespace DeviceSyncUnity
{
    public class TouchesMessage : MessageBase
    {
        public int connectionId;
        public TouchMessage[] touches;
    }
}
