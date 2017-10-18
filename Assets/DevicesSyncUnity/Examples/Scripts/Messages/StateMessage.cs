using DevicesSyncUnity.Messages;

namespace DevicesSyncUnity.Examples.Messages
{
    /// <summary>
    /// Message that contains device's accelerations and acceleration events from current and previous frames.
    /// </summary>
    public class StateMessage : DevicesSyncMessage
    {
        // Properties

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }

        /// <summary>
        /// See <see cref="DevicesSyncMessage.MessageType"/>.
        /// </summary>
        public override short MessageType { get { return Messages.MessageType.State; } }

        // Variables

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public int senderConnectionId;

        /// <summary>
        /// The state to send.
        /// </summary>
        public State state;
    }
}
