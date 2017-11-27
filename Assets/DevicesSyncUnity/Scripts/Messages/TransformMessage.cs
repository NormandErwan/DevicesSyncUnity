using System.Collections.Generic;
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

        public TransformInfo[] transformInfos;

        // Methods

        public void Configure(List<Transform> syncedTransforms, List<float> movementThresholds)
        {
            transformInfos = new TransformInfo[syncedTransforms.Count];
            for (int i = 0; i < transformInfos.Length; i++)
            {
                transformInfos[i] = new TransformInfo();
            }
        }

        public void Update(List<Transform> syncedTransforms, List<float> movementThresholds)
        {
            ShouldBeSynchronized = false;
            for (int i = 0; i < transformInfos.Length; i++)
            {
                transformInfos[i].Update(syncedTransforms[i], movementThresholds[i]);
                ShouldBeSynchronized |= transformInfos[i].ShouldBeSynchronized;
            }
        }

        public void Restore(List<Transform> syncedTransforms)
        {
            for (int i = 0; i < transformInfos.Length; i++)
            {
                transformInfos[i].Restore(syncedTransforms[i]);
            }
        }
    }
}
