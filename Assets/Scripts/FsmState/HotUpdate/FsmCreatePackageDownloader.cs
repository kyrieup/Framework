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

    async UniTask IFsmNode.OnCreate(FsmMachine machine)
    {
        _machine = machine;
        await UniTask.CompletedTask;
    }
    async UniTask IFsmNode.OnEnter()
    {
        GameMain.Instance.TriggerEvent(EventName.ChangeProgress, this);

        await CreateDownloader();
    }
    async UniTask IFsmNode.OnUpdate()
    {
        await UniTask.CompletedTask;
    }
    async UniTask IFsmNode.OnExit()
    {
        await UniTask.CompletedTask;
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
            await _machine.ChangeState<FsmUpdaterDone>();
        }
        else
        {
            // 发现新更新文件后，挂起流程系统
            // 注意：开发者需要在下载前检测磁盘空间不足
            int totalDownloadCount = downloader.TotalDownloadCount;
            long totalDownloadBytes = downloader.TotalDownloadBytes;
            GameMain.Instance.TriggerEvent(EventName.FoundUpdateFiles, this, new Dictionary<string, object> { { "totalDownloadCount", totalDownloadCount }, { "totalDownloadBytes", totalDownloadBytes } });
        }
    }
}