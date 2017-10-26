using System.Collections;
using UnityEngine;

namespace DevicesSyncUnity
{
    /// <summary>
    /// The different interval modes.
    /// </summary>
    public enum SendingMode
    {
        /// <summary>
        /// Sends message at regular time intervals.
        /// </summary>
        TimeInterval,

        /// <summary>
        /// Sends message at regular frame intervals.
        /// </summary>
        FramesInterval
    }

    /// <summary>
    /// Device client sychronize with other devices at regular intervals.
    /// </summary>
    public abstract class DevicesSyncInterval : DevicesSync
    {
        // Editor fields

        [SerializeField]
        [Tooltip("Interval mode to send regularly messages.")]
        private SendingMode sendingMode = SendingMode.FramesInterval;

        [SerializeField]
        [Tooltip("The number of frame between each message in FramesInterval mode.")]
        private uint sendingFramesInterval = 1;

        [SerializeField]
        [Tooltip("The time in seconds between each message in TimeInterval mode.")]
        private float sendingTimeInterval = 0.1f;

        [SerializeField]
        private bool autoStartSending = true;

        // Properties

        /// <summary>
        /// Gets or sets the interval mode to send regularly messages.
        /// </summary>
        public SendingMode SendingMode { get; set; }

        /// <summary>
        /// Gets or sets the number of frame between each message in <see cref="SendingMode.FramesInterval"/> mode.
        /// </summary>
        public uint SendingFramesInterval { get; set; }

        /// <summary>
        /// Gets or sets the time in seconds between each message in <see cref="SendingMode.TimeInterval"/> mode.
        /// </summary>
        public float SendingTimeInterval { get; set; }

        /// <summary>
        /// Gets or sets if the sending coroutine <see cref="SendToServerWithInterval"/> will be started when the
        /// NetworkClient will be ready.
        /// </summary>
        public bool AutoStartSending { get { return autoStartSending; } set { autoStartSending = value; } }

        /// <summary>
        /// Gets if the sending coroutine <see cref="SendToServerWithInterval"/> is started.
        /// </summary>
        public bool SendingIsStarted { get; protected set; }

        // Variables

        protected uint sendingFrameCounter = 0;
        protected float sendingTimer = 0;

        // Methods

        /// <summary>
        /// Starts the sending coroutine <see cref="SendToServerWithInterval"/>.
        /// </summary>
        public void StartSending()
        {
            if (NetworkManager.client != null)
            {
                StartCoroutine(SendToServerWithInterval());
                SendingIsStarted = true;
            }
            else
            {
                throw new System.Exception("Unable to start the sending: there is no NetworkClient.");
            }
        }

        /// <summary>
        /// Starts the sending if <see cref="AutoStartSending"/> is true.
        /// </summary>
        protected override void Start()
        {
            base.Start();

            if (SyncMode != SyncMode.ReceiverOnly && isClient && AutoStartSending)
            {
                StartSending();
            }
        }

        /// <summary>
        /// Updates interval counter each frame with elapsed frames or elapsed time and calls
        /// <see cref="OnSendToServerIntervalIteration(bool)"/>.
        /// </summary>
        protected virtual IEnumerator SendToServerWithInterval()
        {
            while (true)
            {
                bool sendToServerThisFrame = false;
                if (SendingMode == SendingMode.FramesInterval)
                {
                    sendingFrameCounter++;
                    if (sendingFrameCounter >= SendingFramesInterval)
                    {
                        sendingFrameCounter = 0;
                        sendToServerThisFrame = true;
                    }
                }
                else if (SendingMode == SendingMode.TimeInterval)
                {
                    if (Time.unscaledTime - sendingTimer >= SendingTimeInterval)
                    {
                        sendingTimer = Time.unscaledTime;
                        sendToServerThisFrame = true;
                    }
                }

                OnSendToServerIntervalIteration(sendToServerThisFrame);
                yield return null;
            }
        }

        /// <summary>
        /// Updates future message to server and sends it if interval counter is over.
        /// </summary>
        /// <param name="shouldSendThisFrame">If interval counter is over this frame.</param>
        protected abstract void OnSendToServerIntervalIteration(bool shouldSendThisFrame);
    }
}
