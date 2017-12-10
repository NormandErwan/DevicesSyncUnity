using System.Collections.Generic;
using UnityEngine;

namespace DevicesSyncUnity.Messages
{
    /// <summary>
    /// Message that contains the <see cref="Transform.position"/>, <see cref="Transform.rotation"/> and
    /// <see cref="GameObject.activeInHierarchy"/> information of multiple objects.
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

        /// <summary>
        /// True if only one <see cref="TransformInfo.HasChanged"/> is set in <see cref="transformInfos"/>.
        /// </summary>
        public bool HasChanged { get; protected set; }

        // Variables

        /// <summary>
        /// See <see cref="DevicesSyncMessage.SenderConnectionId"/>.
        /// </summary>
        public int senderConnectionId;

        /// <summary>
        /// The position, rotation and object active information of the synchronized objects.
        /// </summary>
        public TransformInfo[] transformInfos;

        // Methods

        /// <summary>
        /// Copies <paramref name="syncedTransforms"/> to <see cref="transformInfos"/> and updates
        /// <see cref="HasChanged"/>.
        /// </summary>
        /// <param name="syncedTransforms"><see cref="TransformSync.SyncedTransforms"/></param>
        /// <param name="movementThreshold"><see cref="TransformSync.MovementThresholdToSync"/></param>
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

            HasChanged = false;
            for (int i = 0; i < transformInfos.Length; i++)
            {
                transformInfos[i].Update(syncedTransforms[i], movementThreshold);
                HasChanged |= transformInfos[i].HasChanged;
            }
        }

        /// <summary>
        /// Updates <paramref name="syncedTransforms"/> with <see cref="transformInfos"/>.
        /// </summary>
        /// <param name="syncedTransforms"><see cref="TransformSync.SyncedTransforms"/></param>
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
