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
        await UniTask.CompletedTask;
    }
    async UniTask IFsmNode.OnEnter()
    {
        GameMain.Instance.TriggerEvent(EventEnum.ChangeProgress, this);

        await BeginDownload();
    }
    async UniTask IFsmNode.OnUpdate()
    {
        await UniTask.CompletedTask;
    }
    async UniTask IFsmNode.OnExit()
    {
        await UniTask.CompletedTask;
    }

    private async UniTask BeginDownload()
    {
        var downloader = (ResourceDownloaderOperation)_machine.GetBlackboardValue("Downloader");
        downloader.OnDownloadErrorCallback = (operation, error) => GameMain.Instance.TriggerEvent(EventEnum.WebFileDownloadFailed, this);
        downloader.OnDownloadProgressCallback = (totalDownloadCount, currentDownloadCount, totalDownloadBytes, currentDownloadBytes) =>
        {
            float progress = (float)currentDownloadBytes / totalDownloadBytes;
            GameMain.Instance.TriggerEvent(EventEnum.DownloadProgressUpdate, this, new System.Collections.Generic.Dictionary<string, object> { { "Progress", progress } });
        };
        downloader.BeginDownload();
        await downloader.ToUniTask();

        // 检测下载结果
        if (downloader.Status != EOperationStatus.Succeed)
            await UniTask.Yield();

        await _machine.ChangeState<FsmDownloadPackageOver>();
    }

}