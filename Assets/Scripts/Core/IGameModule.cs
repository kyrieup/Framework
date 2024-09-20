using Cysharp.Threading.Tasks;

namespace Framework.Core
{
    public interface IGameModule
    {
        string Name { get; }
        UniTask OnInit();
        UniTask OnStart();
        UniTask OnUpdate();
        UniTask OnDestroy();
    }
}