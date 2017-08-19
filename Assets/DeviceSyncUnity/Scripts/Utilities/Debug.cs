using System;
using UnityEngine.Networking;

namespace DeviceSyncUnity
{
    public static class Debug
    {
        public static void Log(string message, int minLogLevel)
        {
            Execute(() => UnityEngine.Debug.Log(message), minLogLevel);
        }

        public static void LogError(string message, int minLogLevel)
        {
            Execute(() => UnityEngine.Debug.LogError(message), minLogLevel);
        }

        public static void Execute(Action action, int minLogLevel)
        {
            if (LogFilter.currentLogLevel <= minLogLevel)
            {
                action();
            }
        }
    }
}
