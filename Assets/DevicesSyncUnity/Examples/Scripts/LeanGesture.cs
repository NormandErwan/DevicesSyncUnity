using UnityEngine;
using System.Collections.Generic;
using DevicesSyncUnity.Examples.Messages;

namespace DevicesSyncUnity.Examples
{
    public static class LeanGesture
    {
        // Gets the average ScreenPosition of the fingers
        public static Vector2 GetScreenCenter(LeanTouchSync leanTouchSync, int deviceId)
        {
            var fingers = new List<LeanFingerInfo>(leanTouchSync.LeanTouches[deviceId].Fingers);
            return GetScreenCenter(fingers);
        }

        public static Vector2 GetScreenCenter(List<LeanFingerInfo> fingers)
        {
            var center = default(Vector2); TryGetScreenCenter(fingers, ref center); return center;
        }

        public static bool TryGetScreenCenter(List<LeanFingerInfo> fingers, ref Vector2 center)
        {
            if (fingers != null)
            {
                var total = Vector2.zero;
                var count = 0;

                for (var i = fingers.Count - 1; i >= 0; i--)
                {
                    var finger = fingers[i];

                    if (finger != null)
                    {
                        total += finger.ScreenPosition;
                        count += 1;
                    }
                }

                if (count > 0)
                {
                    center = total / count; return true;
                }
            }

            return false;
        }

        // Gets the last average ScreenPosition of the fingers
        public static Vector2 GetLastScreenCenter(LeanTouchSync leanTouchSync, int deviceId)
        {
            var fingers = new List<LeanFingerInfo>(leanTouchSync.LeanTouches[deviceId].Fingers);
            return GetLastScreenCenter(fingers);
        }

        public static Vector2 GetLastScreenCenter(List<LeanFingerInfo> fingers)
        {
            var center = default(Vector2); TryGetLastScreenCenter(fingers, ref center); return center;
        }

        public static bool TryGetLastScreenCenter(List<LeanFingerInfo> fingers, ref Vector2 center)
        {
            if (fingers != null)
            {
                var total = Vector2.zero;
                var count = 0;

                for (var i = fingers.Count - 1; i >= 0; i--)
                {
                    var finger = fingers[i];

                    if (finger != null)
                    {
                        total += finger.LastScreenPosition;
                        count += 1;
                    }
                }

                if (count > 0)
                {
                    center = total / count; return true;
                }
            }

            return false;
        }

        // Gets the average ScreenDelta of the fingers
        public static Vector2 GetScreenDelta(LeanTouchSync leanTouchSync, int deviceId)
        {
            var fingers = new List<LeanFingerInfo>(leanTouchSync.LeanTouches[deviceId].Fingers);
            return GetScreenDelta(fingers);
        }

        public static Vector2 GetScreenDelta(List<LeanFingerInfo> fingers)
        {
            var delta = default(Vector2); TryGetScreenDelta(fingers, ref delta); return delta;
        }

        public static bool TryGetScreenDelta(List<LeanFingerInfo> fingers, ref Vector2 delta)
        {
            if (fingers != null)
            {
                var total = Vector2.zero;
                var count = 0;

                for (var i = fingers.Count - 1; i >= 0; i--)
                {
                    var finger = fingers[i];

                    if (finger != null)
                    {
                        total += finger.ScreenDelta;
                        count += 1;
                    }
                }

                if (count > 0)
                {
                    delta = total / count; return true;
                }
            }

            return false;
        }

        // Gets the average ScreenDelta * leanTouchSync.LeanTouchesInfo[deviceId].ScalingFactor of the fingers
        public static Vector2 GetScaledDelta(LeanTouchSync leanTouchSync, int deviceId)
        {
            return GetScreenDelta(leanTouchSync, deviceId) * leanTouchSync.LeanTouchesInfo[deviceId].ScalingFactor;
        }

        public static Vector2 GetScaledDelta(LeanTouchSync leanTouchSync, int deviceId, List<LeanFingerInfo> fingers)
        {
            return GetScreenDelta(fingers) * leanTouchSync.LeanTouchesInfo[deviceId].ScalingFactor;
        }

        public static bool TryGetScaledDelta(LeanTouchSync leanTouchSync, int deviceId, List<LeanFingerInfo> fingers, ref Vector2 delta)
        {
            if (TryGetScreenDelta(fingers, ref delta) == true)
            {
                delta *= leanTouchSync.LeanTouchesInfo[deviceId].ScalingFactor; return true;
            }

            return false;
        }

        // Gets the average WorldDelta of the fingers
        public static Vector3 GetWorldDelta(LeanTouchSync leanTouchSync, int deviceId, float distance, Camera camera = null)
        {
            var fingers = new List<LeanFingerInfo>(leanTouchSync.LeanTouches[deviceId].Fingers);
            return GetWorldDelta(fingers, distance, camera);
        }

        public static Vector3 GetWorldDelta(List<LeanFingerInfo> fingers, float distance, Camera camera = null)
        {
            var delta = default(Vector3); TryGetWorldDelta(fingers, distance, ref delta, camera); return delta;
        }

        public static bool TryGetWorldDelta(List<LeanFingerInfo> fingers, float distance, ref Vector3 delta, Camera camera = null)
        {
            if (Lean.Touch.LeanTouch.GetCamera(ref camera) == true)
            {
                if (fingers != null)
                {
                    var total = Vector3.zero;
                    var count = 0;

                    for (var i = fingers.Count - 1; i >= 0; i--)
                    {
                        var finger = fingers[i];

                        if (finger != null)
                        {
                            total += finger.GetWorldDelta(distance, camera);
                            count += 1;
                        }
                    }

                    if (count > 0)
                    {
                        delta = total / count; return true;
                    }
                }
            }

            return false;
        }

        // Gets the average ScreenPosition distance between the fingers
        public static float GetScreenDistance(LeanTouchSync leanTouchSync, int deviceId)
        {
            var fingers = new List<LeanFingerInfo>(leanTouchSync.LeanTouches[deviceId].Fingers);
            return GetScreenDistance(fingers);
        }

        public static float GetScreenDistance(List<LeanFingerInfo> fingers)
        {
            var distance = default(float);
            var center = default(Vector2);

            if (TryGetScreenCenter(fingers, ref center) == true)
            {
                TryGetScreenDistance(fingers, center, ref distance);
            }

            return distance;
        }

        public static float GetScreenDistance(List<LeanFingerInfo> fingers, Vector2 center)
        {
            var distance = default(float); TryGetScreenDistance(fingers, center, ref distance); return distance;
        }

        public static bool TryGetScreenDistance(List<LeanFingerInfo> fingers, Vector2 center, ref float distance)
        {
            if (fingers != null)
            {
                var total = 0.0f;
                var count = 0;

                for (var i = fingers.Count - 1; i >= 0; i--)
                {
                    var finger = fingers[i];

                    if (finger != null)
                    {
                        total += finger.GetScreenDistance(center);
                        count += 1;
                    }
                }

                if (count > 0)
                {
                    distance = total / count; return true;
                }
            }

            return false;
        }

        // Gets the average ScreenPosition distance * leanTouchSync.LeanTouchesInfo[deviceId].ScalingFactor between the fingers
        public static float GetScaledDistance(LeanTouchSync leanTouchSync, int deviceId)
        {
            return GetScreenDistance(leanTouchSync, deviceId) * leanTouchSync.LeanTouchesInfo[deviceId].ScalingFactor;
        }

        public static float GetScaledDistance(LeanTouchSync leanTouchSync, int deviceId, List<LeanFingerInfo> fingers)
        {
            return GetScreenDistance(fingers) * leanTouchSync.LeanTouchesInfo[deviceId].ScalingFactor;
        }

        public static float GetScaledDistance(LeanTouchSync leanTouchSync, int deviceId, List<LeanFingerInfo> fingers, Vector2 center)
        {
            return GetScreenDistance(fingers, center) * leanTouchSync.LeanTouchesInfo[deviceId].ScalingFactor;
        }

        public static bool TryGetScaledDistance(LeanTouchSync leanTouchSync, int deviceId, List<LeanFingerInfo> fingers, Vector2 center, ref float distance)
        {
            if (TryGetScreenDistance(fingers, center, ref distance) == true)
            {
                distance *= leanTouchSync.LeanTouchesInfo[deviceId].ScalingFactor; return true;
            }

            return false;
        }

        // Gets the last average ScreenPosition distance between all fingers
        public static float GetLastScreenDistance(LeanTouchSync leanTouchSync, int deviceId)
        {
            var fingers = new List<LeanFingerInfo>(leanTouchSync.LeanTouches[deviceId].Fingers);
            return GetLastScreenDistance(fingers);
        }

        public static float GetLastScreenDistance(List<LeanFingerInfo> fingers)
        {
            var distance = default(float);
            var center = default(Vector2);

            if (TryGetLastScreenCenter(fingers, ref center) == true)
            {
                TryGetLastScreenDistance(fingers, center, ref distance);
            }

            return distance;
        }

        public static float GetLastScreenDistance(List<LeanFingerInfo> fingers, Vector2 center)
        {
            var distance = default(float); TryGetLastScreenDistance(fingers, center, ref distance); return distance;
        }

        public static bool TryGetLastScreenDistance(List<LeanFingerInfo> fingers, Vector2 center, ref float distance)
        {
            if (fingers != null)
            {
                var total = 0.0f;
                var count = 0;

                for (var i = fingers.Count - 1; i >= 0; i--)
                {
                    var finger = fingers[i];

                    if (finger != null)
                    {
                        total += finger.GetLastScreenDistance(center);
                        count += 1;
                    }
                }

                if (count > 0)
                {
                    distance = total / count; return true;
                }
            }

            return false;
        }

        // // Gets the last average ScreenPosition distance * leanTouchSync.LeanTouchesInfo[deviceId].ScalingFactor between all fingers
        public static float GetLastScaledDistance(LeanTouchSync leanTouchSync, int deviceId)
        {
            return GetLastScreenDistance(leanTouchSync, deviceId) * leanTouchSync.LeanTouchesInfo[deviceId].ScalingFactor;
        }

        public static float GetLastScaledDistance(LeanTouchSync leanTouchSync, int deviceId, List<LeanFingerInfo> fingers)
        {
            return GetLastScreenDistance(fingers) * leanTouchSync.LeanTouchesInfo[deviceId].ScalingFactor;
        }

        public static float GetLastScaledDistance(LeanTouchSync leanTouchSync, int deviceId, List<LeanFingerInfo> fingers, Vector2 center)
        {
            return GetLastScreenDistance(fingers, center) * leanTouchSync.LeanTouchesInfo[deviceId].ScalingFactor;
        }

        public static bool TryGetLastScaledDistance(LeanTouchSync leanTouchSync, int deviceId, List<LeanFingerInfo> fingers, Vector2 center, ref float distance)
        {
            if (TryGetLastScreenDistance(fingers, center, ref distance) == true)
            {
                distance *= leanTouchSync.LeanTouchesInfo[deviceId].ScalingFactor; return true;
            }

            return false;
        }

        // Gets the pinch scale of the fingers
        public static float GetPinchScale(LeanTouchSync leanTouchSync, int deviceId, float wheelSensitivity = 0.0f)
        {
            var fingers = new List<LeanFingerInfo>(leanTouchSync.LeanTouches[deviceId].Fingers);
            return GetPinchScale(fingers, wheelSensitivity);
        }

        public static float GetPinchScale(List<LeanFingerInfo> fingers, float wheelSensitivity = 0.0f)
        {
            var scale = 1.0f;
            var center = GetScreenCenter(fingers);
            var lastCenter = GetLastScreenCenter(fingers);

            TryGetPinchScale(fingers, center, lastCenter, ref scale, wheelSensitivity);

            return scale;
        }

        public static bool TryGetPinchScale(List<LeanFingerInfo> fingers, Vector2 center, Vector2 lastCenter, ref float scale, float wheelSensitivity = 0.0f)
        {
            var distance = GetScreenDistance(fingers, center);
            var lastDistance = GetLastScreenDistance(fingers, lastCenter);

            if (lastDistance > 0.0f)
            {
                scale = distance / lastDistance; return true;
            }

            if (wheelSensitivity != 0.0f)
            {
                var scroll = Input.mouseScrollDelta.y;

                if (scroll > 0.0f)
                {
                    scale = 1.0f - wheelSensitivity; return true;
                }

                if (scroll < 0.0f)
                {
                    scale = 1.0f + wheelSensitivity; return true;
                }
            }

            return false;
        }

        // Gets the pinch ratio of the fingers (reciprocal of pinch scale)
        public static float GetPinchRatio(LeanTouchSync leanTouchSync, int deviceId, float wheelSensitivity = 0.0f)
        {
            var fingers = new List<LeanFingerInfo>(leanTouchSync.LeanTouches[deviceId].Fingers);
            return GetPinchRatio(fingers, wheelSensitivity);
        }

        public static float GetPinchRatio(List<LeanFingerInfo> fingers, float wheelSensitivity = 0.0f)
        {
            var ratio = 1.0f;
            var center = GetScreenCenter(fingers);
            var lastCenter = GetLastScreenCenter(fingers);

            TryGetPinchRatio(fingers, center, lastCenter, ref ratio, wheelSensitivity);

            return ratio;
        }

        public static bool TryGetPinchRatio(List<LeanFingerInfo> fingers, Vector2 center, Vector2 lastCenter, ref float ratio, float wheelSensitivity = 0.0f)
        {
            var distance = GetScreenDistance(fingers, center);
            var lastDistance = GetLastScreenDistance(fingers, lastCenter);

            if (distance > 0.0f)
            {
                ratio = lastDistance / distance;

                return true;
            }

            if (wheelSensitivity != 0.0f)
            {
                var scroll = Input.mouseScrollDelta.y;

                if (scroll > 0.0f)
                {
                    ratio = 1.0f + wheelSensitivity; return true;
                }

                if (scroll < 0.0f)
                {
                    ratio = 1.0f - wheelSensitivity; return true;
                }
            }

            return false;
        }

        // Gets the average twist of the fingers in degrees
        public static float GetTwistDegrees(LeanTouchSync leanTouchSync, int deviceId)
        {
            var fingers = new List<LeanFingerInfo>(leanTouchSync.LeanTouches[deviceId].Fingers);
            return GetTwistDegrees(fingers);
        }

        public static float GetTwistDegrees(List<LeanFingerInfo> fingers)
        {
            return GetTwistRadians(fingers) * Mathf.Rad2Deg;
        }

        public static float GetTwistDegrees(List<LeanFingerInfo> fingers, Vector2 center, Vector2 lastCenter)
        {
            return GetTwistRadians(fingers, center, lastCenter) * Mathf.Rad2Deg;
        }

        public static bool TryGetTwistDegrees(List<LeanFingerInfo> fingers, Vector2 center, Vector2 lastCenter, ref float degrees)
        {
            if (TryGetTwistRadians(fingers, center, lastCenter, ref degrees) == true)
            {
                degrees *= Mathf.Rad2Deg;

                return true;
            }

            return false;
        }

        // Gets the average twist of the fingers in radians
        public static float GetTwistRadians(LeanTouchSync leanTouchSync, int deviceId)
        {
            var fingers = new List<LeanFingerInfo>(leanTouchSync.LeanTouches[deviceId].Fingers);
            return GetTwistRadians(fingers);
        }

        public static float GetTwistRadians(List<LeanFingerInfo> fingers)
        {
            var center = LeanGesture.GetScreenCenter(fingers);
            var lastCenter = LeanGesture.GetLastScreenCenter(fingers);

            return GetTwistRadians(fingers, center, lastCenter);
        }

        public static float GetTwistRadians(List<LeanFingerInfo> fingers, Vector2 center, Vector2 lastCenter)
        {
            var radians = default(float); TryGetTwistRadians(fingers, center, lastCenter, ref radians); return radians;
        }

        public static bool TryGetTwistRadians(List<LeanFingerInfo> fingers, Vector2 center, Vector2 lastCenter, ref float radians)
        {
            if (fingers != null)
            {
                var total = 0.0f;
                var count = 0;

                for (var i = fingers.Count - 1; i >= 0; i--)
                {
                    var finger = fingers[i];

                    if (finger != null)
                    {
                        total += finger.GetDeltaRadians(center, lastCenter);
                        count += 1;
                    }
                }

                if (count > 0)
                {
                    radians = total / count; return true;
                }
            }

            return false;
        }
    }
}