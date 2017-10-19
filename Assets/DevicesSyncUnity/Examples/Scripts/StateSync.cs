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

        // Methods

        /// <summary>
        /// Initializes the properties.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            MessageTypes.Add(latestStateMessage.MessageType);
        }

        /// <summary>
        /// Send to other devices the new state.
        /// </summary>
        /// <param name="newState">The new state to synchronise.</param>
        protected virtual void SendNewState(State newState)
        {
            if (newState != latestStateMessage.state)
            {
                SendToServer(new StateMessage() { state = newState });
            }
        }

        /// <summary>
        /// Calls <see cref="ProcessMessageReceived(NetworkMessage)"/>.
        /// </summary>
        protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
        {
            return ProcessMessageReceived(netMessage);
        }

        /// <summary>
        /// Calls <see cref="ProcessMessageReceived(NetworkMessage)"/>.
        /// </summary>
        protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
        {
            return ProcessMessageReceived(netMessage);
        }

        /// <summary>
        /// Updates <see cref="CurrentState"/> and invokes <see cref="CurrentStateUpdated"/>.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected virtual DevicesSyncMessage ProcessMessageReceived(NetworkMessage netMessage)
        {
            latestStateMessage = netMessage.ReadMessage<StateMessage>();
            CurrentStateUpdated.Invoke(latestStateMessage);
            return latestStateMessage;
        }

        /// <summary>
        /// See <see cref="DevicesSync.OnClientDeviceConnected(int)"/>.
        /// </summary>
        /// <param name="deviceId"></param>
        protected override void OnClientDeviceConnected(int deviceId)
        {
            SendToClient(deviceId, latestStateMessage);
        }

        /// <summary>
        /// See <see cref="DevicesSync.OnClientDeviceDisconnected(int)"/>.
        /// </summary>=
        protected override void OnClientDeviceDisconnected(int deviceId)
        {
        }
    }
}
