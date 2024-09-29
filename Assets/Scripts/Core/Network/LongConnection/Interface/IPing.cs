using System;
public interface IPing : INetworkPlugin
{
    int Ping { get; }

    void AcceptPacket(object packet);
}