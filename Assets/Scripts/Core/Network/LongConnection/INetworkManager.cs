using System;
using System.Collections.Generic;

public interface INetworkManager
{
    /// <summary>
    /// 获取网络频道数量。
    /// </summary>
    int NetworkChannelCount
    {
        get;
    }

    /// <summary>
    /// 检查是否存在网络频道。
    /// </summary>
    /// <param name="name">网络频道名称。</param>
    /// <returns>是否存在网络频道。</returns>
    bool HasNetworkChannel(string name);

    /// <summary>
    /// 获取网络频道。
    /// </summary>
    /// <param name="name">网络频道名称。</param>
    /// <returns>要获取的网络频道。</returns>
    INetworkChannel GetNetworkChannel(string name);

    /// <summary>
    /// 获取所有网络频道。
    /// </summary>
    /// <returns>所有网络频道。</returns>
    INetworkChannel[] GetAllNetworkChannels();

    /// <summary>
    /// 获取所有网络频道。
    /// </summary>
    /// <param name="results">所有网络频道。</param>
    void GetAllNetworkChannels(List<INetworkChannel> results);

    /// <summary>
    /// 创建网络频道。
    /// </summary>
    /// <param name="name">网络频道名称。</param>
    /// <param name="serviceType">网络服务类型。</param>
    /// <param name="networkChannelHelper">网络频道辅助器。</param>
    /// <returns>要创建的网络频道。</returns>
    INetworkChannel CreateNetworkChannel(string name, EServiceType serviceType, INetworkChannelHelper networkChannelHelper);

    /// <summary>
    /// 销毁网络频道。
    /// </summary>
    /// <param name="name">网络频道名称。</param>
    /// <returns>是否销毁网络频道成功。</returns>
    bool DestroyNetworkChannel(string name);
}