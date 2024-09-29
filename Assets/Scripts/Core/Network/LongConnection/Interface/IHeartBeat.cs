using System;
public interface IHeartBeat : INetworkPlugin
{

    void Reset();

    bool MissHeartBeat(int count);
}
