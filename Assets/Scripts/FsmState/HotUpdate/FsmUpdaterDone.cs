using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Core;
using Cysharp.Threading.Tasks;
/// <summary>
/// 流程更新完毕
/// </summary>
internal class FsmUpdaterDone : IFsmNode
{
    private FsmMachine _machine;

    async UniTask IFsmNode.OnCreate(FsmMachine machine)
    {
        _machine = machine;
        await UniTask.CompletedTask;
    }
    async UniTask IFsmNode.OnEnter()
    {
        await UniTask.CompletedTask;
    }
    async UniTask IFsmNode.OnUpdate()
    {
        await UniTask.CompletedTask;
    }
    async UniTask IFsmNode.OnExit()
    {
        GameMain.Instance.TriggerEvent(EventEnum.UpdaterDone, null);
        await UniTask.CompletedTask;
    }
}