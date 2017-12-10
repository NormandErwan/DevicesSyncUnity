using System.Collections.Generic;
using UnityEngine;

namespace DevicesSyncUnity.Messages
{
    /// <summary>
    /// Message that contains <see cref="Transform.position"/> and <see cref="Transform.rotation"/> information of
    /// multiple transforms.
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

        /// <summary>
        /// The position and rotation information of the synchronized transforms.
        /// </summary>
        public TransformInfo[] transformInfos;

        // Methods

        public void Update(List<Transform> syncedTransforms, float movementThreshold)
        {
            if (transformInfos == null || transformInfos.Length != syncedTransforms.Count)
            {
                transformInfos = new TransformInfo[syncedTransforms.Count];
                for (int i = 0; i < transformInfos.Length; i++)
                {
                    transformInfos[i] = new TransformInfo();
                }
            }

            ShouldBeSynchronized = false;
            for (int i = 0; i < transformInfos.Length; i++)
            {
                transformInfos[i].Update(syncedTransforms[i], movementThreshold);
                ShouldBeSynchronized |= transformInfos[i].HasMoved;
            }
        }

        public void Restore(List<Transform> syncedTransforms)
        {
            for (int i = 0; i < transformInfos.Length; i++)
            {
                if (syncedTransforms.Count > i)
                {
                    transformInfos[i].Restore(syncedTransforms[i]);
                }
            }
        }
    }
}
