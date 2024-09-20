using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Core;
using Cysharp.Threading.Tasks;

/// <summary>
/// 下载完毕
/// </summary>
internal class FsmDownloadPackageOver : IFsmNode
{
    private FsmMachine _machine;

    async UniTask IFsmNode.OnCreate(FsmMachine machine)
    {
        _machine = machine;
    }
    async UniTask IFsmNode.OnEnter()
    {
        _machine.ChangeState<FsmClearPackageCache>();
    }
    async UniTask IFsmNode.OnUpdate()
    {
    }
    async UniTask IFsmNode.OnExit()
    {
    }

}