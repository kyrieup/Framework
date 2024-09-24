using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
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
            await UniTask.CompletedTask;
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

        public async UniTask ChangeState<T>()
        {
            await _fsmMachine.ChangeState<T>();
        }

        public async UniTask ChangeState(string name)
        {
            await _fsmMachine.ChangeState(name);
        }

        public IFsmNode GetCurrentState()
        {
            return _fsmMachine.GetCurrentState();
        }
    }
}