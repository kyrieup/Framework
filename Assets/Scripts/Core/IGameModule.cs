namespace Framework.Core
{
    public interface IGameModule
    {
        string Name { get; }
        void OnInit();
        void OnStart();
        void OnUpdate();
        void OnDestroy();
    }
}