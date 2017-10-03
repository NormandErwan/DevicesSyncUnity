using UnityEngine.Networking;

namespace DevicesSyncUnity.Messages
{
    /// <summary>
    /// Container class for networking messages ids used in <see cref="DevicesSync"/>.
    /// </summary>
    public class MessageType : MsgType
    {
        // Constants

        /// <summary>
        /// The smallest value of this serie of networking message ids. All of theses ids are based on this value.
        /// Update it to shift these ids.
        /// </summary>
        public static short Smallest { get { return smallest; } set { smallest = value; } }

        /// <summary>
        /// Networking message for communicating <see cref="DeviceInfoMessage"/>.
        /// </summary>
        public static short DeviceInfo { get { return Smallest; } }

        /// <summary>
        /// Networking message used by server for communicating to clients another client has disconnected.
        /// </summary>
        public static short DeviceDisconnected { get { return (short)(Smallest + 1); } }

        /// <summary>
        /// Networking message for communicating <see cref="TouchesMessage"/>.
        /// </summary>
        public static short Touches { get { return (short)(Smallest + 2); } }

        /// <summary>
        /// Networking message for communicating <see cref="AccelerationEventsMessage"/>.
        /// </summary>
        public static short AccelerationEvents { get { return (short)(Smallest + 3); } }

        /// <summary>
        /// The highest value of this serie of networking message ids. Additional ids must be above this value.
        /// </summary>
        public static new short Highest { get { return AccelerationEvents; } }

        // Variables

        private static short smallest = MsgType.Highest + 1;

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
