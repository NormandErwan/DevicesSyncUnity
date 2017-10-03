using Lean.Touch;
using UnityEngine;

namespace DevicesSyncUnity.Examples.Messages
{
    /// <summary>
    /// Copy of <see cref="LeanFinger"/> class, usable for network messages.
    /// </summary>
    public class LeanFingerInfo
    {
        // Variables

        public int Index;
        public float Age;
        public bool Set;
        public bool LastSet;
        public bool Tap;
        public int TapCount;
        public bool Swipe;
        public Vector2 StartScreenPosition;
        public Vector2 LastScreenPosition;
        public Vector2 ScreenPosition;
        public bool StartedOverGui;
        //public LeanSnapshot[] Snapshots; // TODO: create a LeanSnapshotInfo class

        public bool IsActive;
        //public float SnapshotDuration; // TODO: replace LeanSnapshot
        public bool IsOverGui;
        public bool Down;
        public bool Up;
        public Vector2 LastSnapshotScreenDelta;
        public Vector2 LastSnapshotScaledDelta;
        public Vector2 ScreenDelta;
        public Vector2 ScaledDelta;
        public Vector2 SwipeScreenDelta;
        public Vector2 SwipeScaledDelta;

        private LeanFinger leanFinger;
        private LeanTouchInfoMessage leanTouchInfo;

        // Methods

        // TODO: replace LeanTouch.GetCamera
        /*public Ray GetRay(Camera camera = null)
        {
            return leanFinger.GetRay(camera);
        }

        public Ray GetStartRay(Camera camera = null)
        {
            return leanFinger.GetStartRay(camera);
        }*/

        // TODO: replace LeanSnapshot.TryGetScreenPosition
        /*public Vector2 GetSnapshotScreenDelta(float deltaTime)
        {
            return leanFinger.GetSnapshotScreenDelta(deltaTime);
        }

        public Vector2 GetSnapshotScaledDelta(float deltaTime)
        {
            return leanFinger.GetSnapshotScaledDelta(deltaTime);
        }

        public Vector2 GetSnapshotScreenPosition(float targetAge)
        {
            return leanFinger.GetSnapshotScreenPosition(targetAge);
        }

        public Vector3 GetSnapshotWorldPosition(float targetAge, float distance, Camera camera = null)
        {
            return leanFinger.GetSnapshotWorldPosition(targetAge, distance, camera);
        }*/

        public float GetRadians(Vector2 referencePoint)
        {
            return leanFinger.GetRadians(referencePoint);
        }

        public float GetDegrees(Vector2 referencePoint)
        {
            return leanFinger.GetDegrees(referencePoint);
        }

        public float GetLastRadians(Vector2 referencePoint)
        {
            return leanFinger.GetLastRadians(referencePoint);
        }

        public float GetLastDegrees(Vector2 referencePoint)
        {
            return leanFinger.GetLastDegrees(referencePoint);
        }

        public float GetDeltaRadians(Vector2 referencePoint)
        {
            return leanFinger.GetDeltaRadians(referencePoint);
        }

        public float GetDeltaRadians(Vector2 referencePoint, Vector2 lastReferencePoint)
        {
            return leanFinger.GetDeltaRadians(referencePoint, lastReferencePoint);
        }

        public float GetDeltaDegrees(Vector2 referencePoint)
        {
            return leanFinger.GetDeltaDegrees(referencePoint);
        }

        public float GetDeltaDegrees(Vector2 referencePoint, Vector2 lastReferencePoint)
        {
            return leanFinger.GetDeltaDegrees(referencePoint, lastReferencePoint);
        }

        public float GetScreenDistance(Vector2 point)
        {
            return leanFinger.GetScreenDistance(point);
        }

        public float GetScaledDistance(Vector2 point)
        {
            return GetScreenDistance(point) * leanTouchInfo.ScalingFactor;
        }

        public float GetLastScreenDistance(Vector2 point)
        {
            return leanFinger.GetLastScreenDistance(point);
        }

        public float GetLastScaledDistance(Vector2 point)
        {
            return GetLastScreenDistance(point) * leanTouchInfo.ScalingFactor;
        }

        // TODO: replace LeanTouch.GetCamera
        /*public Vector3 GetStartWorldPosition(float distance, Camera camera = null)
        {
            return leanFinger.GetStartWorldPosition(distance, camera);
        }

        public Vector3 GetLastWorldPosition(float distance, Camera camera = null)
        {
            return leanFinger.GetLastWorldPosition(distance, camera);
        }

        public Vector3 GetWorldPosition(float distance, Camera camera = null)
        {
            return leanFinger.GetWorldPosition(distance, camera);
        }

        public Vector3 GetWorldDelta(float distance, Camera camera = null)
        {
            return leanFinger.GetWorldDelta(distance, camera);
        }

        public Vector3 GetWorldDelta(float lastDistance, float distance, Camera camera = null)
        {
            return leanFinger.GetWorldDelta(lastDistance, distance, camera);
        }*/

        // TODO
        /*public void ClearSnapshots(int count = -1)
        {
            leanFinger.ClearSnapshots(count);
        }*/

        // TODO
        /*public void RecordSnapshot()
        {
            leanFinger.RecordSnapshot();
        }*/

        /// <summary>
        /// Returns a LeanFingerInfo with values copied from a <see cref="LeanFinger"/> instance.
        /// </summary>
        public static implicit operator LeanFingerInfo(LeanFinger leanFinger)
        {
            return new LeanFingerInfo()
            {
                Index = leanFinger.Index,
                Age = leanFinger.Age,
                Set = leanFinger.Set,
                LastSet = leanFinger.LastSet,
                Tap = leanFinger.Tap,
                TapCount = leanFinger.TapCount,
                Swipe = leanFinger.Swipe,
                StartScreenPosition = leanFinger.StartScreenPosition,
                LastScreenPosition = leanFinger.LastScreenPosition,
                ScreenPosition = leanFinger.ScreenPosition,
                StartedOverGui = leanFinger.StartedOverGui,
                IsActive = leanFinger.IsActive,
                IsOverGui = leanFinger.IsOverGui,
                Down = leanFinger.Down,
                Up = leanFinger.Up,
                LastSnapshotScreenDelta = leanFinger.LastSnapshotScreenDelta,
                LastSnapshotScaledDelta = leanFinger.LastSnapshotScaledDelta,
                ScreenDelta = leanFinger.ScreenDelta,
                ScaledDelta = leanFinger.ScaledDelta,
                SwipeScreenDelta = leanFinger.SwipeScreenDelta,
                SwipeScaledDelta = leanFinger.SwipeScaledDelta,
                leanFinger = leanFinger
            };
        }

        internal void RestoreInfo(LeanTouchInfoMessage leanTouchInfo)
        {
            leanFinger = new LeanFinger()
            {
                Index = this.Index,
                Age = this.Age,
                Set = this.Set,
                LastSet = this.LastSet,
                Tap = this.Tap,
                TapCount = this.TapCount,
                Swipe = this.Swipe,
                StartScreenPosition = this.StartScreenPosition,
                LastScreenPosition = this.LastScreenPosition,
                ScreenPosition = this.ScreenPosition,
                StartedOverGui = this.StartedOverGui
            };
            this.leanTouchInfo = leanTouchInfo;
        }
    }
}
