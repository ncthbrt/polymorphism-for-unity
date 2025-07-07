#nullable enable
using System;
using UnityEngine;

namespace Polymorphism4Unity.Editor
{
    /// <summary>
    /// Wrapper to allow centralised configuration of loggers in this package.
    /// Mirrors the <see cref="ILogger"/> <see langword="interface" /> except all fields and methods are <see langword="static" />.
    /// </summary>
    public static class LoggerProvider
    {
        private static ILogger? currentLogger = Debug.unityLogger;
        public static ILogger Logger { set => currentLogger = value; }
        public static ILogHandler? logHandler
        {
            get => currentLogger?.logHandler;
            set
            {
                if (currentLogger is not null)
                {
                    currentLogger.logHandler = value;
                }
            }
        }

        public static bool logEnabled
        {
            get => currentLogger?.logEnabled ?? false;
            set
            {
                if (currentLogger is not null)
                {
                    currentLogger.logEnabled = value;
                }
            }
        }

        public static LogType filterLogType
        {
            get => currentLogger?.filterLogType ?? LogType.Log;
            set
            {
                if (currentLogger is not null)
                {
                    currentLogger.filterLogType = value;
                }
            }
        }


        public static bool IsLogTypeAllowed(LogType logType)
        {
            return currentLogger?.IsLogTypeAllowed(logType) ?? false;
        }

        public static void Log(LogType logType, object message)
        {
            currentLogger?.Log(logType, message);
        }

        public static void Log(LogType logType, object message, UnityEngine.Object context)
        {
            currentLogger?.Log(logType, message, context);
        }

        public static void Log(LogType logType, string tag, object message)
        {
            currentLogger?.Log(logType, tag, message);
        }

        public static void Log(LogType logType, string tag, object message, UnityEngine.Object context)
        {
            currentLogger?.Log(logType, tag, message, context);
        }

        public static void Log(object message)
        {
            currentLogger?.Log(message);
        }

        public static void Log(string tag, object message)
        {
            currentLogger?.Log(tag, message);
        }

        public static void Log(string tag, object message, UnityEngine.Object context)
        {
            currentLogger?.Log(tag, message, context);
        }

        public static void LogError(string tag, object message)
        {
            currentLogger?.LogError(tag, message);
        }

        public static void LogError(string tag, object message, UnityEngine.Object context)
        {
            currentLogger?.LogError(tag, message, context);
        }

        public static void LogException(Exception exception)
        {
            currentLogger?.LogException(exception);
        }

        public static void LogException(Exception exception, UnityEngine.Object context)
        {
            currentLogger?.LogException(exception, context);
        }

        public static void LogFormat(LogType logType, string format, params object[] args)
        {
            currentLogger?.LogFormat(logType, format, args);
        }

        public static void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            currentLogger?.LogFormat(logType, context, format, args);
        }

        public static void LogWarning(string tag, object message)
        {
            currentLogger?.LogWarning(tag, message);
        }

        public static void LogWarning(string tag, object message, UnityEngine.Object context)
        {
            currentLogger?.LogWarning(tag, message, context);
        }
    }
}