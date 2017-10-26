using UnityEngine;

namespace DevicesSyncUnity.Messages
{
    /// <summary>
    /// Message that contains device static <see cref="Input"/> and <see cref="Camera.main"/> information.
    /// </summary>
    public class TransformMessage : DevicesSyncMessage
    {
        // Properties

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public override int SenderConnectionId { get { return senderConnectionId; } set { senderConnectionId = value; } }

        /// <summary>
        /// See <see cref="DevicesSyncMessage.MessageType"/>.
        /// </summary>
        public override short MessageType { get { return Messages.MessageType.Transform; } }

        public bool ShouldBeSynchronized { get; protected set; }

        // Variables

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public int senderConnectionId;

        public TransformInfo transformInfo;

        // Methods

        public void Update(Transform transform, float movementThreshold)
        {
            if (transformInfo == null)
            {
                transformInfo = new TransformInfo();
            }
            transformInfo.Update(transform, movementThreshold);
            ShouldBeSynchronized = transformInfo.ShouldBeSynchronized;
        }

        public void Restore(Transform transform)
        {
            transformInfo.Restore(transform);
        }
    }
}
