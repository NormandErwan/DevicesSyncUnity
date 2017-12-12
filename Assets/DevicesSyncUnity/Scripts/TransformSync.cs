using DevicesSyncUnity.Messages;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DevicesSyncUnity
{
    /// <summary>
    /// Synchronize multiple objects' <see cref="Transform"/> between devices with <see cref="TransformMessage"/>.
    /// </summary>
    public class TransformSync : DevicesSyncInterval
    {
        // Editor fields

        [SerializeField]
        [Tooltip("The list of transforms (position and rotation) to sync.")]
        private Transform[] syncedTransforms;

        [SerializeField]
        [Tooltip("The minimum movement that any transform must have done to sync. In meters for position and degrees" +
            "for rotation.")]
        private float movementThresholdToSync = 0.001f;

        // Properties

        /// <summary>
        /// Gets the list of <see cref="Transform"/> to synchronize.
        /// </summary>
        public List<Transform> SyncedTransforms { get; protected set; }

        /// <summary>
        /// Gets or sets the minimum movement that any transform must have done since the last send to sync
        /// <see cref="SyncedTransforms"/>. In meters for position and degrees for rotation.
        /// </summary>
        public float MovementThresholdToSync { get { return movementThresholdToSync; } set { movementThresholdToSync = value; } }

        // Variables

        protected TransformMessage currentMessage = new TransformMessage();

        // Events

        /// <summary>
        /// Called on server and on device client when the <see cref="SyncedTransforms"/> have been updated.
        /// </summary>
        public event Action<TransformSync> SyncedTransformsUpdated = delegate { };

        // Methods

        /// <summary>
        /// Initializes properties.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            SyncedTransforms = new List<Transform>(syncedTransforms);
            MessageTypes.Add(currentMessage.MessageType);
        }

        /// <summary>
        /// Updates the transform message, and send it if one of the synced transform has changed since the previous send.
        /// </summary>
        /// <param name="shouldSendThisFrame">If the transform information must be updated this frame.</param>
        protected override void OnSendToServerIntervalIteration(bool shouldSendThisFrame)
        {
            if (shouldSendThisFrame)
            {
                currentMessage.Update(SyncedTransforms, MovementThresholdToSync);
                if (currentMessage.HasChanged)
                {
                    SendToServer(currentMessage);
                }
            }
        }

        /// <summary>
        /// Calls on server <see cref="TransformMessage.Restore(List{Transform})"/> on the received message with
        /// <see cref="SyncedTransforms"/> and calls the <see cref="SyncedTransformsUpdated"/> event.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
        {
            var transformMessage = netMessage.ReadMessage<TransformMessage>();
            transformMessage.Restore(SyncedTransforms);
            SyncedTransformsUpdated(this);
            return transformMessage;
        }

        /// <summary>
        /// Calls on device client <see cref="TransformMessage.Restore(List{Transform})"/> on the received message with
        /// <see cref="SyncedTransforms"/> and calls the <see cref="SyncedTransformsUpdated"/> event.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
        {
            var transformMessage = netMessage.ReadMessage<TransformMessage>();
            if (!isServer)
            {
                transformMessage.Restore(SyncedTransforms);
                currentMessage.Update(SyncedTransforms, MovementThresholdToSync);
                SyncedTransformsUpdated(this);
            }
            return transformMessage;
        }
    }
}
