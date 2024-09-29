using System;

public interface INetworkPlugin
{
    void OnUpdate(float deltaTime, float unscaleTime);

    void SetNetChannel(INetChannel netChannel);
}