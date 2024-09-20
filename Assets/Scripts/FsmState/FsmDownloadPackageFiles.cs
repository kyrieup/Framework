using System.Collections;
using UnityEngine;
using Framework.Core;
using YooAsset;
using Cysharp.Threading.Tasks;

/// <summary>
/// 下载更新文件
/// </summary>
internal class FsmDownloadPackageFiles : IFsmNode
{
    private FsmMachine _machine;

    async UniTask IFsmNode.OnCreate(FsmMachine machine)
    {
        _machine = machine;
    }
    async UniTask IFsmNode.OnEnter()
    {
        GameEntry.Instance.GetEventModule().TriggerEvent(EventEnum.ChangeProgress, this);

        await BeginDownload();
    }
    async UniTask IFsmNode.OnUpdate()
    {
    }
    async UniTask IFsmNode.OnExit()
    {
    }

    private async UniTask BeginDownload()
    {
        var downloader = (ResourceDownloaderOperation)_machine.GetBlackboardValue("Downloader");
        downloader.OnDownloadErrorCallback = (operation, error) => GameEntry.Instance.GetEventModule().TriggerEvent(EventEnum.WebFileDownloadFailed, this);
        downloader.OnDownloadProgressCallback = (totalDownloadCount, currentDownloadCount, totalDownloadBytes, currentDownloadBytes) =>
        {
            float progress = (float)currentDownloadBytes / totalDownloadBytes;
            GameEntry.Instance.GetEventModule().TriggerEvent(EventEnum.DownloadProgressUpdate, this, new System.Collections.Generic.Dictionary<string, object> { { "Progress", progress } });
        };
        downloader.BeginDownload();
        await downloader.ToUniTask();

        // 检测下载结果
        if (downloader.Status != EOperationStatus.Succeed)
            await UniTask.Yield();

        _machine.ChangeState<FsmDownloadPackageOver>();
    }

}