using Framework.Core;
using Cysharp.Threading.Tasks;



public class FsmInitialize : IFsmNode
{
    private FsmMachine _machine;

    async UniTask IFsmNode.OnCreate(FsmMachine machine)
    {
        _machine = machine;
        await UniTask.CompletedTask;
    }
    async UniTask IFsmNode.OnEnter()
    {
        await UniTask.CompletedTask;
    }
    async UniTask IFsmNode.OnUpdate()
    {
        await _machine.ChangeState<FsmHotUpdate>();
    }
    async UniTask IFsmNode.OnExit()
    {
        await UniTask.CompletedTask;
    }
}