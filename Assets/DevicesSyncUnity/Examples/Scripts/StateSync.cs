using DevicesSyncUnity.Examples.Messages;
using DevicesSyncUnity.Messages;
using System;
using UnityEngine.Networking;

namespace DevicesSyncUnity.Examples
{
    /// <summary>
    /// Synchronize a <see cref="State"/> between devices with <see cref="StateMessage"/>.
    /// </summary>
    public class StateSync : DevicesSync
    {
        // Properties

        /// <summary>
        /// Gets the current synchronized state.
        /// </summary>
        public State CurrentState { get { return latestStateMessage.state; } set { SendNewState(value); } }

        // Events

        /// <summary>
        /// Called on server and on device client when <see cref="CurrentState"/> has been updated by a device.
        /// </summary>
        public event Action<StateMessage> CurrentStateUpdated = delegate { };

        // Variables

        protected StateMessage latestStateMessage = new StateMessage();
        protected StateMessage stateMessageToSend = new StateMessage();

        // Methods

        /// <summary>
        /// Initializes the properties and susbcribes to events.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            DeviceConnected += DevicesInfoSync_DeviceConnected;

            MessageTypes.Add(latestStateMessage.MessageType);
        }

        /// <summary>
        /// Unsubscribes to events.
        /// </summary>
        protected virtual void OnDestroy()
        {
            DeviceConnected -= DevicesInfoSync_DeviceConnected;
        }

        /// <summary>
        /// Send to other devices the new state.
        /// </summary>
        /// <param name="newState">The new state to synchronise.</param>
        protected virtual void SendNewState(State newState)
        {
            if (newState != latestStateMessage.state)
            {
                stateMessageToSend.state = newState;
                SendToServer(stateMessageToSend);
            }
        }

        /// <summary>
        /// Updates <see cref="CurrentState"/> and invokes <see cref="CurrentStateUpdated"/>.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
        {
            latestStateMessage = netMessage.ReadMessage<StateMessage>();
            CurrentStateUpdated.Invoke(latestStateMessage);
            return latestStateMessage;
        }

        /// <summary>
        /// Updates <see cref="CurrentState"/> and invokes <see cref="CurrentStateUpdated"/>.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
        {
            latestStateMessage = netMessage.ReadMessage<StateMessage>();
            if (isServer)
            {
                CurrentStateUpdated.Invoke(latestStateMessage);
            }
            return latestStateMessage;
        }

        /// <summary>
        /// Server sends to the new device client the current state.
        /// </summary>
        /// <param name="deviceId">The new device client id.</param>
        protected virtual void DevicesInfoSync_DeviceConnected(int deviceId)
        {
            if (isServer)
            {
                SendToClient(deviceId, latestStateMessage);
            }
        }
    }
}
