using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Framework.Core
{
    public class EventArgs
    {
        /// <summary>
        /// 事件名称
        /// </summary>
        public string EventName { get; private set; }
        /// <summary>
        /// 事件发送者
        /// </summary>
        public object Sender { get; private set; }
        /// <summary>
        /// 事件参数
        /// </summary>
        public Dictionary<string, object> Parameters { get; private set; }

        public EventArgs(string eventName, object sender, Dictionary<string, object> parameters = null)
        {
            EventName = eventName;
            Sender = sender;
            Parameters = parameters ?? new Dictionary<string, object>();
        }
    }

    public class EventModule : IGameModule
    {
        public string Name => "Event";

        private Dictionary<string, Action<EventArgs>> eventDictionary;

        public EventModule()
        {
            eventDictionary = new Dictionary<string, Action<EventArgs>>();
        }

        public async UniTask OnInit() { await UniTask.CompletedTask; }
        public async UniTask OnStart() { await UniTask.CompletedTask; }
        public async UniTask OnUpdate() { await UniTask.CompletedTask; }
        public async UniTask OnDestroy()
        {
            eventDictionary.Clear();
            await UniTask.CompletedTask;
        }

        public void AddListener(string eventName, Action<EventArgs> listener)
        {
            if (!eventDictionary.ContainsKey(eventName))
            {
                eventDictionary[eventName] = listener;
            }
            else
            {
                eventDictionary[eventName] += listener;
            }
        }

        public void RemoveListener(string eventName, Action<EventArgs> listener)
        {
            if (eventDictionary.ContainsKey(eventName))
            {
                eventDictionary[eventName] -= listener;

                if (eventDictionary[eventName] == null)
                {
                    eventDictionary.Remove(eventName);
                }
            }
        }

        public void TriggerEvent(string eventName, object sender, Dictionary<string, object> parameters = null)
        {
            if (eventDictionary.ContainsKey(eventName))
            {
                EventArgs args = new EventArgs(eventName, sender, parameters);
                eventDictionary[eventName]?.Invoke(args);
            }
        }
    }

    /// <summary>
    /// 事件枚举
    /// </summary>
    public static class EventName
    {
        /// <summary>
        /// 进度改变
        /// </summary>
        public const string ChangeProgress = "ChangeProgress";
        /// <summary>
        /// 初始化失败
        /// </summary>
        public const string InitializeFailed = "InitializeFailed";
        /// <summary>
        /// 发现更新文件
        /// </summary>
        public const string FoundUpdateFiles = "FoundUpdateFiles";
        /// <summary>
        /// 下载失败
        /// </summary>
        public const string WebFileDownloadFailed = "WebFileDownloadFailed";
        /// <summary>
        /// 下载进度更新
        /// </summary>
        public const string DownloadProgressUpdate = "DownloadProgressUpdate";
        /// <summary>
        /// 更新包版本失败
        /// </summary>
        public const string PatchManifestUpdateFailed = "PatchManifestUpdateFailed";
        /// <summary>
        /// 包版本更新失败
        /// </summary>
        public const string PackageVersionUpdateFailed = "PackageVersionUpdateFailed";
        /// <summary>
        /// 更新完毕
        /// </summary>
        public const string UpdaterDone = "UpdaterDone";
        /// <summary>
        /// 网络消息
        /// </summary>
        public const string NetworkMessage = "NetworkMessage";
        /// <summary>
        /// 网络频道错误
        /// </summary>
        public const string NetworkChannelError = "NetworkChannelError";
        /// <summary>
        /// 网络错误
        /// </summary>
        public const string NetworkCustomError = "NetworkCustomError";
        /// <summary>
        /// 网络连接成功
        /// </summary>
        public const string NetworkConnected = "NetworkConnected";
        /// <summary>
        /// 网络连接失败
        /// </summary>
        public const string NetworkConnectFailed = "NetworkConnectFailed";
        /// <summary>
        /// 网络连接断开
        /// </summary>
        public const string NetworkDisconnected = "NetworkDisconnected";
        /// <summary>
        /// 网络心跳丢失
        /// </summary>
        public const string NetworkHeartBeatLost = "NetworkHeartBeatLost";  
    }
}