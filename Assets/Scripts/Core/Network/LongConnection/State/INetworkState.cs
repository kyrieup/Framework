using System.IO;
interface INetworkState
    {
        void Enter(NetworkManager networkManager);
        void Connect(string host, int port, IProtocolResolver protocolResolver);
        void Disconnect();
        void Reconnect();
        void Send(MemoryStream packet);

    }
