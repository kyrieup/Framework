using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Framework.Core;
using YooAsset;

public class HotUpdateModule : IGameModule
{

    private enum HotUpdateSteps
    {
        None,
        Update,
        Done,
    }
    public string Name => "HotUpdateModel";

    public EPlayMode CurrentMode { get; set; }

    // 当前版本号
    public string CurrentVersion { get; set; }

    // 下载器
    public ResourceDownloaderOperation Downloader { get; set; }

    private HotUpdateSteps _steps = HotUpdateSteps.None;
    private FsmMachine _fsmMachine;

    public async UniTask OnInit()
    {
        // 添加事件监听
        AddListener();
        // 初始化FSM
        InitFsm();
        await UniTask.CompletedTask;
    }

    public async UniTask OnStart()
    {
        // 可以在这里添加启动时的逻辑
        await UniTask.CompletedTask;
    }

    public async UniTask OnDestroy()
    {
        // 可以在这里添加销毁时的清理逻辑
        await UniTask.CompletedTask;
    }

    private void InitFsm()
    {
        _fsmMachine = new FsmMachine();
        _fsmMachine.AddNode<FsmInitializePackage>();
        _fsmMachine.AddNode<FsmUpdatePackageVersion>();
        _fsmMachine.AddNode<FsmUpdatePackageManifest>();
        _fsmMachine.AddNode<FsmCreatePackageDownloader>();
        _fsmMachine.AddNode<FsmDownloadPackageFiles>();
        _fsmMachine.AddNode<FsmDownloadPackageOver>();
        _fsmMachine.AddNode<FsmClearPackageCache>();
        _fsmMachine.AddNode<FsmUpdaterDone>();
        _fsmMachine.SetBlackboardValue("PackageName", "DefaultPackage");
        _fsmMachine.SetBlackboardValue("PlayMode", EPlayMode.EditorSimulateMode);
        _fsmMachine.SetBlackboardValue("BuildPipeline", EDefaultBuildPipeline.BuiltinBuildPipeline.ToString());
        _steps = HotUpdateSteps.None;
    }

    public async UniTask OnUpdate()
    {
        if (_steps == HotUpdateSteps.Update)
        {
            await _fsmMachine.OnUpdate();
        }
    }

    public async UniTask StartHotUpdate()
    {
        _steps = HotUpdateSteps.Update;
        await _fsmMachine.ChangeState<FsmInitializePackage>();
    }

    private void AddListener()
    {
        GameMain.Instance.AddListener(EventName.UpdaterDone, (args) =>
        {
            _steps = HotUpdateSteps.Done;
        });
    }

}