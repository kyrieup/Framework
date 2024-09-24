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
            await UniTask.CompletedTask;
        }

        public async UniTask OnStart()
        {
            if (_curNode != null)
            {
                await _curNode.OnEnter();
            }
        }

        public async UniTask OnUpdate()
        {
            if (_curNode != null)
            {
                await _curNode.OnUpdate();
            }
        }

        public async UniTask OnDestroy()
        {
            states.Clear();
            _curNode = null;
            await UniTask.CompletedTask;
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

        public async UniTask ChangeState<T>()
        {
            await ChangeState(typeof(T).FullName);
        }

        public async UniTask ChangeState(string name)
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
                if (_preNode != null)
                {
                    await _preNode.OnExit();
                }
                Debug.Log($"从{(_preNode != null ? _preNode.GetType().Name : "null")}转换到{_curNode.GetType().Name}");
                await _curNode.OnEnter();
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