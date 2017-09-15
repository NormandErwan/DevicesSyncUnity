using UnityEngine.Networking;

namespace DeviceSyncUnity.Messages
{
    public class MessageType
    {
        public const short DeviceInfo = MsgType.Highest + 1;
        public const short DeviceDisconnected = MsgType.Highest + 2;
        public const short Touches = MsgType.Highest + 3;
        public const short Acceleration = MsgType.Highest + 4;
    }
}
