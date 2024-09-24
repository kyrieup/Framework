using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Core;
using YooAsset;
using Cysharp.Threading.Tasks;

/// <summary>
/// 清理未使用的缓存文件
/// </summary>
internal class FsmClearPackageCache : IFsmNode
{
    private FsmMachine _machine;

    async UniTask IFsmNode.OnCreate(FsmMachine machine)
    {
        _machine = machine;
        await UniTask.CompletedTask;
    }
    async UniTask IFsmNode.OnEnter()
    {
        GameMain.Instance.TriggerEvent(EventEnum.ChangeProgress, this);

        var packageName = (string)_machine.GetBlackboardValue("PackageName");
        var package = YooAssets.GetPackage(packageName);
        var operation = package.ClearUnusedBundleFilesAsync();
        operation.Completed += Operation_Completed;
        await operation;
    }
    async UniTask IFsmNode.OnUpdate()
    {
        await UniTask.CompletedTask;
    }
    async UniTask IFsmNode.OnExit()
    {
        await UniTask.CompletedTask;
    }

    private async void Operation_Completed(YooAsset.AsyncOperationBase obj)
    {
        await _machine.ChangeState<FsmUpdaterDone>();
    }
}