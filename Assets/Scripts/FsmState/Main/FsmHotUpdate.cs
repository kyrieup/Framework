using Framework.Core;
using Cysharp.Threading.Tasks;




public class FsmHotUpdate : IFsmNode
{
    private FsmMachine _machine;

    async UniTask IFsmNode.OnCreate(FsmMachine machine)
    {
        _machine = machine;
        await GameMain.Instance.GetModule<HotUpdateModule>().StartHotUpdate();   

    }
    async UniTask IFsmNode.OnEnter()
    {
        await UniTask.CompletedTask;
    }
    async UniTask IFsmNode.OnUpdate()
    {
        await UniTask.CompletedTask;
    }
    async UniTask IFsmNode.OnExit()
    {
        await UniTask.CompletedTask;
    }
}
