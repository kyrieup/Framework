using UnityEngine;
using System;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
namespace Framework.Core
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Fatal
    }

    public class DebugConfig
    {
        /// <summary>
        /// 日志前缀
        /// </summary>
        public string logHeadFix = "###";

        /// <summary>
        /// 文件储存路径
        /// </summary>
        public string logFileSavePath { get { return Application.persistentDataPath + "/"; } }

        /// <summary>
        /// 当前日志级别
        /// </summary>
        public LogLevel currentLogLevel = LogLevel.Debug;

        /// <summary>
        /// 日志文件名称
        /// </summary>
        public string logFileName { get { return Application.productName + " " + DateTime.Now.ToString("yyyy-MM-dd HH-mm") + ".log"; } }
    }

    public enum LogColor
    {
        /// <summary>
        /// 默认颜色
        /// </summary>
        None,
        /// <summary>
        /// 蓝色
        /// </summary>
        Blue,
        /// <summary>
        /// 青色
        /// </summary>
        Cyan,
        /// <summary>
        /// 深蓝色
        /// </summary>
        Darkblue,
        /// <summary>
        /// 绿色
        /// </summary>
        Green,
        /// <summary>
        /// 灰色
        /// </summary>
        Grey,
        /// <summary>
        /// 橙黄色
        /// </summary>
        Orange,
        /// <summary>
        /// 紫色
        /// </summary>
        Purple,
        /// <summary>
        /// 洋红色
        /// </summary>
        Magenta,
        /// <summary>
        /// 红色
        /// </summary>
        Red,
        /// <summary>
        /// 黄色
        /// </summary>
        Yellow,
    }

    public class DebugModule : IGameModule
    {
        public string Name => "Debug";

        public DebugConfig config = new DebugConfig();

        public async UniTask OnInit()
        {
#if OPEN_LOG
            SRDebug.Init();
            UnityEngine.Debug.unityLogger.logEnabled = true;
#else
            // 关闭日志系统
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
#endif
        }

        public async UniTask OnStart()
        {
            
        }

        public async UniTask OnUpdate()
        {
            Log("DebugModule OnUpdate", LogLevel.Debug, LogColor.Blue);
        }

        public async UniTask OnDestroy()
        {

        }

        [Conditional("OPEN_LOG")]
        public void Log(string message, LogLevel level = LogLevel.Debug, LogColor color = LogColor.None)
        {
            if (level >= config.currentLogLevel)
            {
                string formattedMessage = FormatLogMessage(message, level, color);

                // 根据日志级别输出到Unity控制台
                switch (level)
                {
                    case LogLevel.Debug:
                    case LogLevel.Info:
                        UnityEngine.Debug.Log(formattedMessage);
                        break;
                    case LogLevel.Warning:
                        UnityEngine.Debug.LogWarning(formattedMessage);
                        break;
                    case LogLevel.Error:
                    case LogLevel.Fatal:
                        UnityEngine.Debug.LogError(formattedMessage);
                        break;
                }

            }
        }

        private string FormatLogMessage(string message, LogLevel level, LogColor color)
        {
            return GetUnityColor($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}", color);
        }

        private string GetUnityColor(string msg, LogColor color)
        {
            if (color == LogColor.None)
            {
                return msg;
            }
            switch (color)
            {
                case LogColor.Blue:
                    msg = $"<color=#0000FF>{msg}</color>";
                    break;
                case LogColor.Cyan:
                    msg = $"<color=#00FFFF>{msg}</color>";
                    break;
                case LogColor.Darkblue:
                    msg = $"<color=#8FBC8F>{msg}</color>";
                    break;
                case LogColor.Green:
                    msg = $"<color=#00FF00>{msg}</color>";
                    break;
                case LogColor.Orange:
                    msg = $"<color=#FFA500>{msg}</color>";
                    break;
                case LogColor.Red:
                    msg = $"<color=#FF0000>{msg}</color>";
                    break;
                case LogColor.Yellow:
                    msg = $"<color=#FFFF00>{msg}</color>";
                    break;
                case LogColor.Magenta:
                    msg = $"<color=#FF00FF>{msg}</color>";
                    break;
            }
            return msg;
        }
    }
}