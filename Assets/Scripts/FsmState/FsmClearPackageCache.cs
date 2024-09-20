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
    }
    async UniTask IFsmNode.OnEnter()
    {
        PatchEventDefine.PatchStatesChange.SendEventMessage("清理未使用的缓存文件！");
        var packageName = (string)_machine.GetBlackboardValue("PackageName");
        var package = YooAssets.GetPackage(packageName);
        var operation = package.ClearUnusedBundleFilesAsync();
        operation.Completed += Operation_Completed;
    }
    async UniTask IFsmNode.OnUpdate()
    {
    }
    async UniTask IFsmNode.OnExit()
    {
    }

    private void Operation_Completed(YooAsset.AsyncOperationBase obj)
    {
        _machine.ChangeState<FsmUpdaterDone>();
    }

    public UniTask OnCreate(FsmMachine machine)
    {
        throw new System.NotImplementedException();
    }

    public UniTask OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public UniTask OnUpdate()
    {
        throw new System.NotImplementedException();
    }

    public UniTask OnExit()
    {
        throw new System.NotImplementedException();
    }
}