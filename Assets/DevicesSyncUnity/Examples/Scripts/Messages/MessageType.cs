using UnityEngine.Networking;

namespace DevicesSyncUnity.Examples.Messages
{
    /// <summary>
    /// Container class for networking messages types used in <see cref="LeanTouchSync"/>.
    /// </summary>
    public class MessageType : DevicesSyncUnity.Messages.MessageType
    {
        // Constants

        /// <summary>
        /// See <see cref="DevicesSyncUnity.Messages.MessageType.Smallest"/>.
        /// </summary>
        public static new short Smallest { get { return smallest; } set { smallest = value; } }

        /// <summary>
        /// Networking message for communicating <see cref="LeanTouchInfoMessage"/>.
        /// </summary>
        public static short LeanTouchInfo { get { return Smallest; } }

        /// <summary>
        /// Networking message for communicating <see cref="LeanTouchMessage"/>.
        /// </summary>
        public static short LeanTouch { get { return (short)(Smallest + 1); } }

        /// <summary>
        /// See <see cref="DevicesSyncUnity.Messages.MessageType.Highest"/>.
        /// </summary>
        public static new short Highest { get { return LeanTouch; } }

        // Variables

        private static short smallest = (short)(DevicesSyncUnity.Messages.MessageType.Highest + 1);

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
