using System;
using System.Text;

namespace DebugToolkit.Console.Log
{
    public static class DebugLog
    {
        public static event Action<string, LogType> OnLog;

        public enum LogColor
        {
            Default,
            Red,
            Green,
            Blue,
            Yellow,
            White
        }

        public enum LogType
        {
            Log,
            Warning,
            Error,
            Command
        }

        public static void Log(int i, LogColor logColor = LogColor.Default, LogType logType = LogType.Log)
        {
            Log(i.ToString(), logColor, logType);
        }

        public static void Log(bool b, LogColor logColor = LogColor.Default, LogType logType = LogType.Log)
        {
            Log(b.ToString(), logColor, logType);
        }

        public static void Log(string message, LogColor logColor = LogColor.Default, LogType logType = LogType.Log)
        {
            StringBuilder log = new StringBuilder();

            log.Append(">_");
            
            if (logColor == LogColor.Default)
            {
                switch (logType)
                {
                    case LogType.Log:
                        log.Append("<color=#78C2C1>").Append(message).Append("</color>");
                        break;
                    case LogType.Warning:
                        log.Append("<color=#FFCC6C>").Append(message).Append("</color>");
                        break;
                    case LogType.Error:
                        log.Append("<color=#FF746C>").Append(message).Append("</color>");
                        break;
                    case LogType.Command:
                        log.Append("<color=#6BEDFF>").Append(message).Append("</color>");
                        break;
                }

            }
            else
                switch (logColor)
                {
                    case LogColor.Red:
                        log.Append("<color=#FF746C>").Append(message).Append("</color>");
                        break;
                    case LogColor.Green:
                        log.Append("<color=green>").Append(message).Append("</color>");
                        break;
                    case LogColor.Blue:
                        log.Append("<color=#78C2C1>").Append(message).Append("</color>");
                        break;
                    case LogColor.Yellow:
                        log.Append("<color=#FFCC6C>").Append(message).Append("</color>");
                        break;
                    case LogColor.White:
                        log.Append("<color=#F7FFFF>").Append(message).Append("</color>");
                        break;
                }

            OnLog?.Invoke(log.ToString(), logType);
        }
    }
}
