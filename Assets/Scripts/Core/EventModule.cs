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
        public EventEnum EventName { get; private set; }
        /// <summary>
        /// 事件发送者
        /// </summary>
        public object Sender { get; private set; }
        /// <summary>
        /// 事件参数
        /// </summary>
        public Dictionary<string, object> Parameters { get; private set; }

        public EventArgs(EventEnum eventName, object sender, Dictionary<string, object> parameters = null)
        {
            EventName = eventName;
            Sender = sender;
            Parameters = parameters ?? new Dictionary<string, object>();
        }
    }

    public class EventModule : IGameModule
    {
        public string Name => "Event";

        private Dictionary<EventEnum, Action<EventArgs>> eventDictionary;

        public EventModule()
        {
            eventDictionary = new Dictionary<EventEnum, Action<EventArgs>>();
        }

        public async UniTask OnInit() { }
        public async UniTask OnStart() { }
        public async UniTask OnUpdate() { }
        public async UniTask OnDestroy() 
        {
            eventDictionary.Clear();
        }

        public void AddListener(EventEnum eventName, Action<EventArgs> listener)
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

        public void RemoveListener(EventEnum eventName, Action<EventArgs> listener)
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

        public void TriggerEvent(EventEnum eventName, object sender, Dictionary<string, object> parameters = null)
        {
            if (eventDictionary.ContainsKey(eventName))
            {
                EventArgs args = new EventArgs(eventName, sender, parameters);
                eventDictionary[eventName]?.Invoke(args);
            }
        }
    }

    public enum EventEnum
    {
        ChangeProgress,
        InitializeFailed,
        FoundUpdateFiles,
        WebFileDownloadFailed,
        DownloadProgressUpdate,
        PatchManifestUpdateFailed,
        PackageVersionUpdateFailed,
    }
}