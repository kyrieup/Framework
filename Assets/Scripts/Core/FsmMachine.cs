using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Framework.Core
{
    public class FsmMachine
    {
        private Dictionary<string, IFsmNode> states = new Dictionary<string, IFsmNode>();

        private readonly Dictionary<string, System.Object> _blackboard = new Dictionary<string, object>(100);
        private IFsmNode _curNode;
        private IFsmNode _preNode;

        public async UniTask OnInit()
        {
            states = new Dictionary<string, IFsmNode>();
        }

        public async UniTask OnStart()
        {
            _curNode?.OnEnter();
        }

        public async UniTask OnUpdate()
        {
            _curNode?.OnUpdate();
        }

        public async UniTask OnDestroy()
        {
            states.Clear();
            _curNode = null;
        }

        public void AddNode<T>() where T : IFsmNode
        {
            AddNode(Activator.CreateInstance(typeof(T)) as IFsmNode);
        }

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
                node.OnCreate(this);
            }
        }

        public void ChangeState<T>()
        {
            ChangeState(typeof(T).FullName);
        }

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
                Debug.Log($"不存在该节点{name}");
            }
        }

        public IFsmNode GetCurrentState()
        {
            return _curNode;
        }
        /// <summary>
        /// 获取黑板数据
        /// </summary>
        public System.Object GetBlackboardValue(string key)
        {
            if (_blackboard.TryGetValue(key, out System.Object value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 设置黑板数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetBlackboardValue(string key, System.Object value)
        {
            if (_blackboard.ContainsKey(key) == false)
                _blackboard.Add(key, value);
            else
                _blackboard[key] = value;
        }

    }

    public interface IFsmNode
    {
        UniTask OnCreate(FsmMachine machine);
        UniTask OnEnter();
        UniTask OnUpdate();
        UniTask OnExit();
    }
}