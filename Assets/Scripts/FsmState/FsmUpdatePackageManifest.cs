using System.Collections;
using UnityEngine;
using YooAsset;
using Framework.Core;
using Cysharp.Threading.Tasks;

/// <summary>
/// 更新资源清单
/// </summary>
public class FsmUpdatePackageManifest : IFsmNode
{
    private FsmMachine _machine;

    async UniTask IFsmNode.OnCreate(FsmMachine machine)
    {
        _machine = machine;
    }
    async UniTask IFsmNode.OnEnter()
    {
        GameEntry.Instance.GetEventModule().TriggerEvent(EventEnum.ChangeProgress, this);
        await UpdateManifest();
    }
    async UniTask IFsmNode.OnUpdate()
    {
    }
    async UniTask IFsmNode.OnExit()
    {
    }

    private async UniTask UpdateManifest()
    {
        await UniTask.Delay(500);

        var packageName = (string)_machine.GetBlackboardValue("PackageName");
        var packageVersion = (string)_machine.GetBlackboardValue("PackageVersion");
        var package = YooAssets.GetPackage(packageName);
        var operation = package.UpdatePackageManifestAsync(packageVersion);
        await operation;

        if (operation.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(operation.Error);
            GameEntry.Instance.GetEventModule().TriggerEvent(EventEnum.PatchManifestUpdateFailed, this);
            return;
        }
        else
        {
            _machine.ChangeState<FsmCreatePackageDownloader>();
        }
    }
}