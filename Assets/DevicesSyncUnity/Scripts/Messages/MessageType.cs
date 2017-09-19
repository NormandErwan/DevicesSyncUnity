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
        /// Networking message for communicating <see cref="DevicesInfoMessage"/>.
        /// </summary>
        public const short DeviceInfo = Highest + 1;

        /// <summary>
        /// Networking message used by server for communicating to clients another client has disconnected.
        /// </summary>
        public const short DeviceDisconnected = Highest + 2;

        /// <summary>
        /// Networking message for communicating <see cref="TouchesMessage"/>.
        /// </summary>
        public const short Touches = Highest + 3;

        /// <summary>
        /// Networking message for communicating <see cref="AccelerationEventsMessage"/>.
        /// </summary>
        public const short Acceleration = Highest + 4;

        // Methods

        /// <summary>
        /// See <see cref="MsgType.MsgTypeToString(short)"/>.
        /// </summary>
        public static new string MsgTypeToString(short value)
        {
            switch (value)
            {
                case DeviceInfo:
                    return "DeviceInfo";
                case DeviceDisconnected:
                    return "DeviceDisconnected";
                case Touches:
                    return "Touches";
                case Acceleration:
                    return "Acceleration";
                default:
                    return MsgType.MsgTypeToString(value);
            }
        }
    }
}
