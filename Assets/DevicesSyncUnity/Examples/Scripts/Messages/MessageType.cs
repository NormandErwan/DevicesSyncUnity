using UnityEngine.Networking;

namespace DevicesSyncUnity.Examples.Messages
{
    /// <summary>
    /// Container class for networking messages types used in <see cref="DevicesSync"/>.
    /// </summary>
    public class MessageType : DevicesSyncUnity.Messages.MessageType
    {
        // Constants

        public static short LeanTouch { get { return (short)(DevicesSyncUnity.Messages.MessageType.Highest + 1); } }

        public static new short Highest { get { return LeanTouch; } }

        // Methods

        /// <summary>
        /// See <see cref="MsgType.MsgTypeToString(short)"/>.
        /// </summary>
        public static new string MsgTypeToString(short value)
        {
            if (value == LeanTouch)
            {
                return "LeanTouch";
            }
            else
            {
                return MsgType.MsgTypeToString(value);
            }
        }
    }
}
