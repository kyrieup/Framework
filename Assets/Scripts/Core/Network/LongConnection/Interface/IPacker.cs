using System;

public interface IPacker : INetworkPlugin
{
    ArraySegment<byte> Pack(object packet);

    object Unpack(ArraySegment<byte> packet);
}