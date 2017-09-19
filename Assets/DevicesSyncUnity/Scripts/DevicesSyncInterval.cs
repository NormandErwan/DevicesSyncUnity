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
        // Properties

        /// <summary>
        /// Gets or sets the interval mode to send regularly messages.
        /// </summary>
        public abstract SendingMode SendingMode { get; set; }

        /// <summary>
        /// Gets or sets the number of frame between each message in <see cref="SendingMode.FramesInterval"/> mode.
        /// </summary>
        public abstract uint SendingFramesInterval { get; set; }

        /// <summary>
        /// Gets or sets the time in seconds between each message in <see cref="SendingMode.TimeInterval"/> mode.
        /// </summary>
        public abstract float SendingTimeInterval { get; set; }

        // Methods

        /// <summary>
        /// Starts
        /// </summary>
        protected override void Start()
        {
            base.Start();
            if (manager.client != null && isClient && SyncMode != SyncMode.ReceiverOnly)
            {
                StartCoroutine(SendToServerWithInterval());
            }
        }

        /// <summary>
        /// Counts frames or elapsed time each frame and calls <see cref="OnSendToServerIntervalIteration(bool)"/>.
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
        /// Updates future message and sends to server if the interval counter is over.
        /// </summary>
        /// <param name="shouldSendThisFrame">If the interval counter is over this frame.</param>
        protected abstract void OnSendToServerIntervalIteration(bool shouldSendThisFrame);
    }
}
