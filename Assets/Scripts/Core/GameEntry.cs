using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    public class GameEntry : MonoBehaviour
    {
        private static GameEntry instance;
        private Dictionary<string, IGameModule> modules;
        private Dictionary<string, string> test;
        public static GameEntry Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("GameEntry");
                    instance = go.AddComponent<GameEntry>();
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
            InitializeModules();
        }

        private void Start()
        {
            foreach (var module in modules.Values)
            {
                module.OnStart();
            }
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
        public void Log(string message) => GetModule<DebugModule>()?.Log(message);

        // 添加这个便捷方法来获取 EventModule
        public EventModule GetEventModule() => GetModule<EventModule>();
    }
}