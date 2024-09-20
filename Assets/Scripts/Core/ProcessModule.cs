using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Framework.Core
{
    public class ProcessModule : IGameModule
    {
        public string Name => "Process";

        private FsmMachine _fsmMachine;

        public async UniTask OnInit()
        {
            _fsmMachine = new FsmMachine();
            await _fsmMachine.OnInit();
        }

        public async UniTask OnStart()
        {
            await _fsmMachine.OnStart();
        }

        public async UniTask OnUpdate()
        {
            await _fsmMachine.OnUpdate();
        }

        public async UniTask OnDestroy()
        {
            await _fsmMachine.OnDestroy();
        }

        // 暴露FsmMachine的方法
        public void AddNode<T>() where T : IFsmNode
        {
            _fsmMachine.AddNode<T>();
        }

        public void AddNode(IFsmNode node)
        {
            _fsmMachine.AddNode(node);
        }

        public void ChangeState<T>()
        {
            _fsmMachine.ChangeState<T>();
        }

        public void ChangeState(string name)
        {
            _fsmMachine.ChangeState(name);
        }

        public IFsmNode GetCurrentState()
        {
            return _fsmMachine.GetCurrentState();
        }
    }
}