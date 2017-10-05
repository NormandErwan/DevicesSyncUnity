using UnityEngine.Networking;

namespace DevicesSyncUnity.Utilities
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
            if (minLogLevel >= LogFilter.currentLogLevel)
            {
                UnityEngine.Debug.Log(message);
            }
        }

        /// <summary>
        /// Logs an warning message if the level of logging is greater or equal to <see cref="LogFilter.Warn"/>.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        public static void LogWarning(string message)
        {
            if (LogFilter.logWarn)
            {
                UnityEngine.Debug.LogWarning(message);
            }
        }

        /// <summary>
        /// Logs an error message if the level of logging is greater or equal to <see cref="LogFilter.Error"/>.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        public static void LogError(string message)
        {
            if (LogFilter.logError)
            {
                UnityEngine.Debug.LogError(message);
            }
        }
    }
}
