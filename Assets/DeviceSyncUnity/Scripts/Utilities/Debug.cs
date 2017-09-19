using System;
using UnityEngine.Networking;

namespace DeviceSyncUnity.Utilities
{
    /// <summary>
    /// Execute debug functions according to <see cref="LogFilter"/>.
    /// </summary>
    public static class Debug
    {
        /// <summary>
        /// Logs message to the Unity console if there is a minimum logging level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="minLogLevel">The minimum logging level to match to display the message in the Unity console.</param>
        public static void Log(string message, int minLogLevel)
        {
            Execute(() => UnityEngine.Debug.Log(message), minLogLevel);
        }

        /// <summary>
        /// Logs an error message if the level of logging is greater or equal to <see cref="LogFilter.Error"/>.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        public static void LogError(string message)
        {
            Execute(() => UnityEngine.Debug.LogError(message), LogFilter.Error);
        }

        /// <summary>
        /// Executes an action if there is a minimum logging level.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="minLogLevel">The minimum logging level to match to display the message in the Unity console.</param>
        public static void Execute(Action action, int minLogLevel)
        {
            if (LogFilter.currentLogLevel <= minLogLevel)
            {
                action();
            }
        }
    }
}
