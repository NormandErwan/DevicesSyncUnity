using UnityEngine.Networking;

namespace DeviceSyncUnity.Messages
{
    public class SenderInfo
    {
        public int connectionId;

        public void UpdateInfo()
        {
            connectionId = NetworkManager.singleton.client.connection.connectionId;
        }
    }
}
