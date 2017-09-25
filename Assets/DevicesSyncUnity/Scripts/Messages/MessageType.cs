using UnityEngine.Networking;

namespace DevicesSyncUnity.Messages
{
    /// <summary>
    /// Container class for networking messages types used in <see cref="DevicesSync"/>.
    /// </summary>
    public class MessageType : MsgType
    {
        // Constants

        /// <summary>
        /// Networking message for communicating <see cref="DeviceInfoMessage"/>.
        /// </summary>
        public static short DeviceInfo { get { return MsgType.Highest + 1; } }

        /// <summary>
        /// Networking message used by server for communicating to clients another client has disconnected.
        /// </summary>
        public static short DeviceDisconnected { get { return MsgType.Highest + 2; } }

        /// <summary>
        /// Networking message for communicating <see cref="TouchesMessage"/>.
        /// </summary>
        public static short Touches { get { return MsgType.Highest + 3; } }

        /// <summary>
        /// Networking message for communicating <see cref="AccelerationEventsMessage"/>.
        /// </summary>
        public static short AccelerationEvents { get { return MsgType.Highest + 4; } }

        public static new short Highest { get { return AccelerationEvents; } }

        // Methods

        /// <summary>
        /// See <see cref="MsgType.MsgTypeToString(short)"/>.
        /// </summary>
        public static new string MsgTypeToString(short value)
        {
            if (value == DeviceInfo)
            {
                return "DeviceInfo";
            }
            else if (value == DeviceDisconnected)
            {
                return "DeviceDisconnected";
            }
            else if (value == Touches)
            {
                return "Touches";
            }
            else if (value == AccelerationEvents)
            {
                return "Acceleration";
            }
            else
            {
                return MsgType.MsgTypeToString(value);
            }
        }
    }
}
