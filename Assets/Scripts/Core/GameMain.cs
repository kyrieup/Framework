using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using YooAsset;

namespace Framework.Core
{
    public class GameMain : MonoBehaviour
    {
        private static GameMain instance;
        private Dictionary<string, IGameModule> modules;
        private Dictionary<string, string> test;
        public static GameMain Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("GameMain");
                    instance = go.AddComponent<GameMain>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            modules = new Dictionary<string, IGameModule>();
            YooAssets.Initialize();
            InitializeModules();
            InitializeProcess();
        }

        private async UniTaskVoid Start()
        {
            Log("Start", LogLevel.Info, LogColor.Green);
            foreach (var module in modules.Values)
            {
                await module.OnStart();
            }
            await GetModule<ProcessModule>().ChangeState<FsmInitialize>();
        }


        private void Update()
        {
            // Log("Update");
            foreach (var module in modules.Values)
            {
                module.OnUpdate();
            }
        }


        private void OnDestroy()
        {
            foreach (var module in modules.Values)
            {
                module.OnDestroy();
            }
            modules.Clear();
        }

        private void InitializeModules()
        {
            // 在这里初始化和注册所有需要的模块
            // 例如：RegisterModule(new ResourceModule());
            RegisterModule(new DebugModule());
            RegisterModule(new EventModule());
            RegisterModule(new ProcessModule());
            RegisterModule(new HotUpdateModule());
        }

        private void InitializeProcess()
        {
            var processModule = GetModule<ProcessModule>();
            processModule.AddNode<FsmInitialize>();
            processModule.AddNode<FsmHotUpdate>();
            processModule.AddNode<FsmPreload>();
            processModule.AddNode<FsmSceneLoad>();
            // 可能还需要设置状态之间的转换关系
        }

        public void RegisterModule(IGameModule module)
        {
            if (!modules.ContainsKey(module.Name))
            {
                modules.Add(module.Name, module);
                module.OnInit();
            }
        }

        public T GetModule<T>() where T : IGameModule
        {
            foreach (var module in modules.Values)
            {
                if (module is T)
                {
                    return (T)module;
                }
            }
            return default(T);
        }

        // 修改便捷方法来使用 GetModule<DebugModule>()
        public void Log(string message, LogLevel level = LogLevel.Debug, LogColor color = LogColor.None) => GetModule<DebugModule>()?.Log(message, level, color);

        // 添加这个便捷方法来获取 EventModule
        private EventModule GetEventModule() => GetModule<EventModule>();
        public void TriggerEvent(EventEnum eventName, object sender, Dictionary<string, object> parameters = null) => GetEventModule().TriggerEvent(eventName, sender, parameters);
        public void AddListener(EventEnum eventName, Action<EventArgs> listener) => GetEventModule().AddListener(eventName, listener);
        public void RemoveListener(EventEnum eventName, Action<EventArgs> listener) => GetEventModule().RemoveListener(eventName, listener);
    }
}