using UnityEngine.Networking;

namespace DeviceSyncUnity.Messages
{
    public abstract class DevicesSyncMessage : MessageBase
    {
        public abstract int SenderConnectionId { get; set; }
    }
}
