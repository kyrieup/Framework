using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Framework.Core
{
    public class FsmModule : IGameModule
    {
        public string Name => "Fsm";

        /// <summary>
        /// 存放状态机的字典
        /// </summary>
        private Dictionary<string, IFsmNode> states = new Dictionary<string, IFsmNode>();

        /// <summary>
        /// 当前节点
        /// </summary>
        private IFsmNode _curNode;

        /// <summary>
        /// 上一个节点
        /// </summary>
        private IFsmNode _preNode;

        public void OnInit()
        {
            states = new Dictionary<string, IFsmNode>();
        }
        public void OnStart() { }

        public void OnUpdate()
        {
            _curNode?.OnUpdate();
        }

        public void OnDestroy()
        {
            states.Clear();
            _curNode = null;
        }

        /// <summary>
        /// 根据泛型添加状态节点
        /// </summary>
        /// <typeparam name="T">继承接口IFsmNode的类</typeparam>
        public void AddNode<T>() where T : IFsmNode
        {
            AddNode(Activator.CreateInstance(typeof(T)) as IFsmNode);
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="node">节点实例</param>
        public void AddNode(IFsmNode node)
        {
            if (node == null)
            {
                return;
            }
            string name = node.GetType().FullName;
            if (!states.TryAdd(name, node))
            {
                Debug.Log($"已注册该节点 {name}");
            }
            else
            {
                node.OnCreate();
            }
        }

        /// <summary>
        /// 根据泛型切换状态
        /// </summary>
        /// <typeparam name="T">继承接口IFsmNode的类</typeparam>
        public void ChangeState<T>()
        {
            ChangeState(typeof(T).FullName);
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        public void ChangeState(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            IFsmNode res;
            if (states.TryGetValue(name, out res))
            {
                _preNode = _curNode;
                _curNode = res;
                _preNode.OnExit();
                Debug.Log($"从{_preNode.GetType().Name}转换到{_curNode.GetType().Name}");
                _curNode.OnEnter();
            }
            else
            {
                Debug.Log($"不存在改节点{name}");
            }
        }

        public IFsmNode GetCurrentState()
        {
            return _curNode;
        }
    }

    public interface IFsmNode
    {
        UniTask OnCreate();
        UniTask OnEnter();
        UniTask OnUpdate();
        UniTask OnExit();
    }
}