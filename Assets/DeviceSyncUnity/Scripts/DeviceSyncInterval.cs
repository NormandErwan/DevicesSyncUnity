using System.Collections;
using UnityEngine;

namespace DeviceSyncUnity
{
    public enum SendingMode
    {
        TimeInterval,
        FramesInterval
    }

    public abstract class DevicesSyncInterval : DevicesSync
    {
        // Properties

        public abstract SendingMode SendingMode { get; set; }
        public abstract uint SendingFramesInterval { get; set; }
        public abstract float SendingTimeInterval { get; set; }

        // Methods

        protected override void Start()
        {
            base.Start();
            if (manager.client != null && isClient && SyncMode != SyncMode.ReceiverOnly)
            {
                StartCoroutine(SendToServerWithInterval());
            }
        }

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

        protected abstract void OnSendToServerIntervalIteration(bool send);
    }
}
