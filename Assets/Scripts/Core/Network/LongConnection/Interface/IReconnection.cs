using System;
public interface IReconnection : INetworkPlugin
{
    bool Reconnect(Exception lastException);

    void Reconnected();
}
