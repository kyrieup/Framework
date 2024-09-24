using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Core;
using Cysharp.Threading.Tasks;
using YooAsset;

/// <summary>
/// 更新资源版本号
/// </summary>
internal class FsmUpdatePackageVersion : IFsmNode
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
        await UpdatePackageVersion();
    }
    async UniTask IFsmNode.OnUpdate()
    {
        await UniTask.CompletedTask;
    }
    async UniTask IFsmNode.OnExit()
    {
        await UniTask.CompletedTask;
    }

    private async UniTask UpdatePackageVersion()
    {
        await UniTask.Delay(500);

        var packageName = (string)_machine.GetBlackboardValue("PackageName");
        var package = YooAssets.GetPackage(packageName);
        var operation = package.RequestPackageVersionAsync();
        await operation;

        if (operation.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(operation.Error);
            GameMain.Instance.TriggerEvent(EventEnum.PackageVersionUpdateFailed, this);
        }
        else
        {
            Debug.Log($"Request package version : {operation.PackageVersion}");
            _machine.SetBlackboardValue("PackageVersion", operation.PackageVersion);
            await _machine.ChangeState<FsmUpdatePackageManifest>();
        }
    }
}