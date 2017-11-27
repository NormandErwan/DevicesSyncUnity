using DevicesSyncUnity.Messages;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DevicesSyncUnity
{
    /// <summary>
    /// Synchronize <see cref="Transform"/> of an object between devices with <see cref="TransformMessage"/>.
    /// </summary>
    [RequireComponent(typeof(NetworkTransform))]
    public class NetworkTransformSync : DevicesSyncInterval
    {
        // Variables

        protected List<Transform> syncedTransforms;
        protected List<float> movementThresholds;
        protected TransformMessage transformMessage = new TransformMessage();

        // Methods

        /// <summary>
        /// Initializes properties.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            var networkTransform = GetComponent<NetworkTransform>();
            var networkTransformChildren = new List<NetworkTransformChild>(GetComponents<NetworkTransformChild>());

            int memberListsCapacity = 1 + networkTransformChildren.Count;
            syncedTransforms = new List<Transform>(memberListsCapacity);
            movementThresholds = new List<float>(memberListsCapacity);

            syncedTransforms.Add(transform);
            movementThresholds.Add(networkTransform.movementTheshold);
            foreach (var networkTransformChild in networkTransformChildren)
            {
                syncedTransforms.Add(networkTransformChild.target);
                movementThresholds.Add(networkTransformChild.movementThreshold);
            }

            transformMessage.Configure(syncedTransforms, movementThresholds);

            MessageTypes.Add(transformMessage.MessageType);
        }

        protected virtual void OnValidate()
        {
            if (SyncMode == SyncMode.SenderAndReceiver)
            {
                SyncMode = SyncMode.SenderOnly;
            }
        }

        protected override void OnSendToServerIntervalIteration(bool shouldSendThisFrame)
        {
            if (shouldSendThisFrame)
            {
                transformMessage.Update(syncedTransforms, movementThresholds);
                if (transformMessage.ShouldBeSynchronized)
                {
                    SendToServer(transformMessage);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
        {
            var transformMessage = netMessage.ReadMessage<TransformMessage>();
            transformMessage.Restore(syncedTransforms);
            return transformMessage;
        }

        protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
        {
            return null;
        }
    }
}
