using UnityEngine;
using System.Diagnostics;

namespace MKMagicToolkit.Utility
{
    /// <summary>
    /// Custom Unity debug utilities included only in developer builds
    /// </summary>
    public static class CustomDebug
    {
        [Conditional("UNITY_ASSERTIONS")]
        public static void Log(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void Log(object message, Object context)
        {
            UnityEngine.Debug.Log(message, context);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void LogWarning(object message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void LogWarning(object message, Object context)
        {
            UnityEngine.Debug.LogWarning(message, context);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void LogError(object message)
        {
            UnityEngine.Debug.LogError(message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void LogError(object message, Object context)
        {
            UnityEngine.Debug.LogError(message, context);
        }
    }
}