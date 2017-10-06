using DevicesSyncUnity.Examples.Messages;
using DevicesSyncUnity.Messages;
using DevicesSyncUnity.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace DevicesSyncUnity.Examples
{
    /// <summary>
    /// Synchronize <see cref="Lean.Touch.LeanTouch"/> information and events between devices with
    /// <see cref="LeanTouchInfoMessage"/> and <see cref="LeanTouchMessage"/>.
    /// </summary>
    public class LeanTouchSync : DevicesSyncInterval
    {
        // Properties

        /// <summary>
        /// Gets LeanTouch static information from currently connected devices.
        /// </summary>
        public Dictionary<int, LeanTouchInfoMessage> LeanTouchesInfo { get; protected set; }

        /// <summary>
        /// Gets latest LeanTouch information from currently connected devices.
        /// </summary>
        public Dictionary<int, LeanTouchMessage> LeanTouches { get; protected set; }

        // Events

        /// <summary>
        /// Called on server and on device client when a <see cref="LeanTouchInfoMessage"/> is received.
        /// </summary>
        public event Action<LeanTouchInfoMessage> LeanTouchInfoReceived = delegate { };

        /// <summary>
        /// Called on server and on device client when a <see cref="LeanTouchMessage"/> is received.
        /// </summary>
        public event Action<LeanTouchMessage> LeanTouchReceived = delegate { };

        /// <summary>
        /// Called on server and on device client for every <see cref="LeanTouchMessage.FingersDown"/> in a received message.
        /// </summary>
        public event Action<int, LeanFingerInfo> OnFingerDown = delegate { };

        /// <summary>
        /// Called on server and on device client for every <see cref="LeanTouchMessage.FingersSet"/> in a received message.
        /// </summary>
        public event Action<int, LeanFingerInfo> OnFingerSet = delegate { };

        /// <summary>
        /// Called on server and on device client for every <see cref="LeanTouchMessage.FingersUp"/> in a received message.
        /// </summary>
        public event Action<int, LeanFingerInfo> OnFingerUp = delegate { };

        /// <summary>
        /// Called on server and on device client for every <see cref="LeanTouchMessage.FingersTap"/> in a received message.
        /// </summary>
        public event Action<int, LeanFingerInfo> OnFingerTap = delegate { };

        /// <summary>
        /// Called on server and device client for every <see cref="LeanTouchMessage.FingersSwipe"/> in a received message.
        /// </summary>
        public event Action<int, LeanFingerInfo> OnFingerSwipe = delegate { };

        /// <summary>
        /// Called on server and device client for a non-empty <see cref="LeanTouchMessage.Gestures"/> list in a received message.
        /// </summary>
        public event Action<int, List<LeanFingerInfo>> OnGesture = delegate { };

        // Variables

        protected bool initialAutoStartSending;
        protected LeanTouchInfoMessage leanTouchInfoMessage = new LeanTouchInfoMessage();
        protected LeanTouchMessage leanTouchMessage = new LeanTouchMessage();
        protected bool lastLeanTouchMessageEmpty = false;

        // Methods

        /// <summary>
        /// Initializes the properties.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            initialAutoStartSending = AutoStartSending;
            AutoStartSending = false;

            LeanTouchesInfo = new Dictionary<int, LeanTouchInfoMessage>();
            LeanTouches = new Dictionary<int, LeanTouchMessage>();

            MessageTypes.Add(leanTouchInfoMessage.MessageType);
            MessageTypes.Add(leanTouchMessage.MessageType);

            if (LogFilter.logInfo)
            {
                OnFingerDown  += (clientId, finger) => { UnityEngine.Debug.Log("LeanTouchSync: finger " + finger.Index + " down on client " + clientId); };
                OnFingerSet   += (clientId, finger) => { UnityEngine.Debug.Log("LeanTouchSync: finger " + finger.Index + " set on client " + clientId); };
                OnFingerUp    += (clientId, finger) => { UnityEngine.Debug.Log("LeanTouchSync: finger " + finger.Index + " up on client " + clientId); };
                OnFingerTap   += (clientId, finger) => { UnityEngine.Debug.Log("LeanTouchSync: finger " + finger.Index + " tap on client " + clientId); };
                OnFingerSwipe += (clientId, finger) => { UnityEngine.Debug.Log("LeanTouchSync: finger " + finger.Index + " swipe on client " + clientId); };
            }
        }

        /// <summary>
        /// Starts capturing the <see cref="Lean.Touch.LeanTouch"/> events and sends a <see cref="LeanTouchInfoMessage"/> to server.
        /// </summary>
        protected override void Start()
        {
            base.Start();

            leanTouchMessage.SetCapturingEvents(true);

            leanTouchInfoMessage.Update();
            SendToServer(leanTouchInfoMessage, Channels.DefaultReliable);
        }

        /// <summary>
        /// Stop capturing the LeanTouch events.
        /// </summary>
        public override void OnNetworkDestroy()
        {
            base.OnNetworkDestroy();
            leanTouchMessage.SetCapturingEvents(false);
        }

        /// <summary>
        /// Updates a <see cref="LeanTouchMessage"/>, and sends it if required and <see cref="LeanTouchMessage.Fingers"/>
        /// is not empty.
        /// </summary>
        /// <param name="sendToServerThisFrame">If the message should be sent this frame.</param>
        protected override void OnSendToServerIntervalIteration(bool sendToServerThisFrame)
        {
            if (sendToServerThisFrame)
            {
                leanTouchMessage.Update();

                bool emptyLeanTouchMessage = leanTouchMessage.Fingers.Length == 0;
                if (!emptyLeanTouchMessage || !lastLeanTouchMessageEmpty)
                {
                    SendToServer(leanTouchMessage);
                    leanTouchMessage.Reset();
                }
                lastLeanTouchMessageEmpty = emptyLeanTouchMessage;
            }
        }

        /// <summary>
        /// For <see cref="LeanTouchInfoMessage"/>, sends the equivalent information from other devices to the sender.
        /// For <see cref="LeanTouchMessage"/>, calls <see cref="ProcessLeanTouchMessage"/>.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected override DevicesSyncMessage OnServerMessageReceived(NetworkMessage netMessage)
        {
            if (netMessage.msgType == leanTouchMessage.MessageType)
            {
                return ProcessLeanTouchMessage(netMessage);
            }
            else if (netMessage.msgType == leanTouchInfoMessage.MessageType)
            {
                var leanTouchInfoMessage = netMessage.ReadMessage<LeanTouchInfoMessage>();
                foreach (var leanTouchInfo in LeanTouchesInfo)
                {
                    SendToClient(leanTouchInfoMessage.SenderConnectionId, leanTouchInfo.Value);
                }

                LeanTouchesInfo[leanTouchInfoMessage.SenderConnectionId] = leanTouchInfoMessage;
                LeanTouchInfoReceived.Invoke(leanTouchInfoMessage);
                return leanTouchInfoMessage;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// For <see cref="LeanTouchInfoMessage"/>, updates <see cref="LeanTouchesInfo"/> and start sending
        /// <see cref="LeanTouchMessage"/> as the server has received LeanTouchInfoMessage.
        /// For <see cref="LeanTouchMessage"/>, calls <see cref="ProcessLeanTouchMessage"/>.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        /// <returns>The typed network message extracted.</returns>
        protected override DevicesSyncMessage OnClientMessageReceived(NetworkMessage netMessage)
        {
            if (netMessage.msgType == leanTouchMessage.MessageType)
            {
                return ProcessLeanTouchMessage(netMessage);
            }
            else if (netMessage.msgType == leanTouchInfoMessage.MessageType)
            {
                var leanTouchInfoMessage = netMessage.ReadMessage<LeanTouchInfoMessage>();
                LeanTouchesInfo[leanTouchInfoMessage.SenderConnectionId] = leanTouchInfoMessage;
                LeanTouchInfoReceived.Invoke(leanTouchInfoMessage);
                
                if (SyncMode != SyncMode.ReceiverOnly && isClient && initialAutoStartSending && !SendingIsStarted)
                {
                    StartSending();
                }

                return leanTouchInfoMessage;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Removes the disconnected device from <see cref="LeanTouchesInfo"/> and <see cref="LeanTouches"/>.
        /// </summary>
        /// <param name="netMessage">The received networking message.</param>
        protected override void OnClientDeviceDisconnectedReceived(DeviceInfoMessage deviceInfoMessage)
        {
            LeanTouchesInfo.Remove(deviceInfoMessage.SenderConnectionId);
            LeanTouches.Remove(deviceInfoMessage.SenderConnectionId);
        }

        /// <summary>
        /// Updates <see cref="LeanTouches"/> and invokes <see cref="LeanTouchReceived"/>, fingers and gestures related
        /// events.
        /// </summary>
        protected virtual LeanTouchMessage ProcessLeanTouchMessage(NetworkMessage netMessage)
        {
            var leanTouchMessage = netMessage.ReadMessage<LeanTouchMessage>();
            leanTouchMessage.Restore(LeanTouchesInfo[leanTouchMessage.SenderConnectionId]);

            LeanTouches[leanTouchMessage.SenderConnectionId] = leanTouchMessage;
            LeanTouchReceived.Invoke(leanTouchMessage);

            if (leanTouchMessage.Fingers.Length > 0)
            {
                var fingerEvents = new List<Tuple<LeanFingerInfo[], Action<int, LeanFingerInfo>>>
                {
                    new Tuple<LeanFingerInfo[], Action<int, LeanFingerInfo>>(leanTouchMessage.FingersDown, OnFingerDown),
                    new Tuple<LeanFingerInfo[], Action<int, LeanFingerInfo>>(leanTouchMessage.FingersSet, OnFingerSet),
                    new Tuple<LeanFingerInfo[], Action<int, LeanFingerInfo>>(leanTouchMessage.FingersUp, OnFingerUp),
                    new Tuple<LeanFingerInfo[], Action<int, LeanFingerInfo>>(leanTouchMessage.FingersTap, OnFingerTap),
                    new Tuple<LeanFingerInfo[], Action<int, LeanFingerInfo>>(leanTouchMessage.FingersSwipe, OnFingerSwipe)
                };

                foreach (var fingerEvent in fingerEvents)
                {
                    foreach (var finger in fingerEvent.Item1)
                    {
                        fingerEvent.Item2.Invoke(leanTouchMessage.SenderConnectionId, finger);
                    }
                }
                OnGesture.Invoke(leanTouchMessage.SenderConnectionId, new List<LeanFingerInfo>(leanTouchMessage.Gestures));
            }

            return leanTouchMessage;
        }
    }
}
