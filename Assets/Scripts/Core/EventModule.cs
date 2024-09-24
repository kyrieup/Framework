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
        public int EventName { get; private set; }
        /// <summary>
        /// 事件发送者
        /// </summary>
        public object Sender { get; private set; }
        /// <summary>
        /// 事件参数
        /// </summary>
        public Dictionary<string, object> Parameters { get; private set; }

        public EventArgs(int eventName, object sender, Dictionary<string, object> parameters = null)
        {
            EventName = eventName;
            Sender = sender;
            Parameters = parameters ?? new Dictionary<string, object>();
        }
    }

    public class EventModule : IGameModule
    {
        public string Name => "Event";

        private Dictionary<int, Action<EventArgs>> eventDictionary;

        public EventModule()
        {
            eventDictionary = new Dictionary<int, Action<EventArgs>>();
        }

        public async UniTask OnInit() { await UniTask.CompletedTask; }
        public async UniTask OnStart() { await UniTask.CompletedTask; }
        public async UniTask OnUpdate() { await UniTask.CompletedTask; }
        public async UniTask OnDestroy()
        {
            eventDictionary.Clear();
            await UniTask.CompletedTask;
        }

        public void AddListener(EventEnum eventName, Action<EventArgs> listener)
        {
            if (!eventDictionary.ContainsKey((int)eventName))
            {
                eventDictionary[(int)eventName] = listener;
            }
            else
            {
                eventDictionary[(int)eventName] += listener;
            }
        }

        public void RemoveListener(EventEnum eventName, Action<EventArgs> listener)
        {
            if (eventDictionary.ContainsKey((int)eventName))
            {
                eventDictionary[(int)eventName] -= listener;

                if (eventDictionary[(int)eventName] == null)
                {
                    eventDictionary.Remove((int)eventName);
                }
            }
        }

        public void TriggerEvent(EventEnum eventName, object sender, Dictionary<string, object> parameters = null)
        {
            if (eventDictionary.ContainsKey((int)eventName))
            {
                EventArgs args = new EventArgs((int)eventName, sender, parameters);
                eventDictionary[(int)eventName]?.Invoke(args);
            }
        }
    }

    /// <summary>
    /// 事件枚举
    /// </summary>
    public enum EventEnum
    {
        /// <summary>
        /// 进度改变
        /// </summary>
        ChangeProgress,
        /// <summary>
        /// 初始化失败
        /// </summary>
        InitializeFailed,
        /// <summary>
        /// 发现更新文件
        /// </summary>
        FoundUpdateFiles,
        /// <summary>
        /// 下载失败
        /// </summary>
        WebFileDownloadFailed,
        /// <summary>
        /// 下载进度更新
        /// </summary>
        DownloadProgressUpdate,
        /// <summary>
        /// 更新包版本失败
        /// </summary>
        PatchManifestUpdateFailed,
        /// <summary>
        /// 包版本更新失败
        /// </summary>
        PackageVersionUpdateFailed,
        /// <summary>
        /// 更新完毕
        /// </summary>
        UpdaterDone,
    }
}