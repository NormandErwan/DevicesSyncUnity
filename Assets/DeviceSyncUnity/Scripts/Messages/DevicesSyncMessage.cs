using UnityEngine.Networking;

namespace DeviceSyncUnity.Messages
{
    public abstract class DevicesSyncMessage : MessageBase
    {
        public abstract SenderInfo SenderInfo { get; set; }

        public virtual void UpdateInfo()
        {
            if (SenderInfo == null)
            {
                SenderInfo = new SenderInfo();
                SenderInfo.UpdateInfo();
            }
        }
    }
}
