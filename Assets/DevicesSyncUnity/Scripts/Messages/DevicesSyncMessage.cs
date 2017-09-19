using UnityEngine.Networking;

namespace DevicesSyncUnity.Messages
{
    /// <summary>
    /// Base class for messages used by <see cref="DevicesSync"/>.
    /// </summary>
    public abstract class DevicesSyncMessage : MessageBase
    {
        /// <summary>
        /// Gets or sets the connection id of the original device client that sent the message.
        /// </summary>
        public abstract int SenderConnectionId { get; set; }
    }
}
