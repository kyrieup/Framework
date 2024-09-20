using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Framework.Core;
using YooAsset;

public class HotUpdateModel : IGameModule
{
    public static HotUpdateModel Instance { get; private set; }

    public string Name => "HotUpdateModel";

    public EPlayMode CurrentMode { get; set; }

    // 当前版本号
    public string CurrentVersion { get; set; }

    // 下载器
    public ResourceDownloaderOperation Downloader { get; set; }

    private bool _isRunning = false;
    private FsmMachine _fsmMachine;

    public async UniTask OnInit()
    {
        Instance = this;
        // 初始化FSM
        InitFsm();
    }

    public async UniTask OnStart()
    {
        // 可以在这里添加启动时的逻辑
    }

    public async UniTask OnDestroy()
    {
        // 可以在这里添加销毁时的清理逻辑
    }

    private void InitFsm()
    {
        _fsmMachine = new FsmMachine();
        _isRunning = true;
    }

    public async UniTask OnUpdate()
    {
        if (_isRunning)
        {
            _fsmMachine.OnUpdate();
        }
    }

}