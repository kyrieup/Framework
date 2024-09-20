using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Core;
using YooAsset;
using Cysharp.Threading.Tasks;

/// <summary>
/// 创建文件下载器
/// </summary>
public class FsmCreatePackageDownloader : IFsmNode
{
    private FsmMachine _machine;

    async UniTask OnCreate(FsmMachine machine)
    {
        _machine = machine;
    }
    async UniTask OnEnter()
    {
        GameEntry.Instance.GetEventModule().TriggerEvent(EventEnum.ChangeProgress, this);

        await CreateDownloader();
    }
    async UniTask OnUpdate()
    {
    }
    async UniTask OnExit()
    {
    }

    async UniTask CreateDownloader()
    {
        await new WaitForSecondsRealtime(0.5f);

        var packageName = (string)_machine.GetBlackboardValue("PackageName");
        var package = YooAssets.GetPackage(packageName);
        int downloadingMaxNum = 10;
        int failedTryAgain = 3;
        var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
        _machine.SetBlackboardValue("Downloader", downloader);

        if (downloader.TotalDownloadCount == 0)
        {
            Debug.Log("Not found any download files !");
            _machine.ChangeState<FsmUpdaterDone>();
        }
        else
        {
            // 发现新更新文件后，挂起流程系统
            // 注意：开发者需要在下载前检测磁盘空间不足
            int totalDownloadCount = downloader.TotalDownloadCount;
            long totalDownloadBytes = downloader.TotalDownloadBytes;
            GameEntry.Instance.GetEventModule().TriggerEvent(EventEnum.FoundUpdateFiles, this, new Dictionary<string, object> { { "totalDownloadCount", totalDownloadCount }, { "totalDownloadBytes", totalDownloadBytes } });
        }
    }

    UniTask IFsmNode.OnCreate(FsmMachine machine)
    {
        throw new System.NotImplementedException();
    }

    UniTask IFsmNode.OnEnter()
    {
        throw new System.NotImplementedException();
    }

    UniTask IFsmNode.OnUpdate()
    {
        throw new System.NotImplementedException();
    }

    UniTask IFsmNode.OnExit()
    {
        throw new System.NotImplementedException();
    }
}