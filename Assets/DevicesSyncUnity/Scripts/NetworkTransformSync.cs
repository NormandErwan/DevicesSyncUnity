using DevicesSyncUnity.Messages;
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

        protected NetworkTransform networkTransform;
        protected TransformMessage transformMessage = new TransformMessage();

        // Methods

        /// <summary>
        /// Initializes properties.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            networkTransform = GetComponent<NetworkTransform>();

            MessageTypes.Add(transformMessage.MessageType);
        }

        protected virtual void OnValidate()
        {
            if (SyncMode == SyncMode.SenderAndReceiver)
            {
                SyncMode = SyncMode.SenderOnly;
            }
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                transform.Rotate(new Vector3(30f, 0f, 0f));
            }
        }

        protected override void OnSendToServerIntervalIteration(bool shouldSendThisFrame)
        {
            if (shouldSendThisFrame)
            {
                transformMessage.Update(transform, networkTransform.movementTheshold);
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
            transformMessage.Restore(transform);
            return transformMessage;
        }

        protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
        {
            return null;
        }
    }
}
