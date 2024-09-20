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
    }
    async UniTask IFsmNode.OnEnter()
    {
        GameEntry.Instance.GetEventModule().TriggerEvent(EventEnum.ChangeProgress, this);
        await UpdatePackageVersion();
    }
    async UniTask IFsmNode.OnUpdate()
    {
    }
    async UniTask IFsmNode.OnExit()
    {
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
            GameEntry.Instance.GetEventModule().TriggerEvent(EventEnum.PackageVersionUpdateFailed, this);
        }
        else
        {
            Debug.Log($"Request package version : {operation.PackageVersion}");
            _machine.SetBlackboardValue("PackageVersion", operation.PackageVersion);
            _machine.ChangeState<FsmUpdatePackageManifest>();
        }
    }
}