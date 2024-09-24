using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Framework.Core;
/// <summary>
/// 网络管理器。
/// </summary>
internal sealed partial class NetworkManager : IGameModule, INetworkManager
{
    private readonly Dictionary<string, NetworkChannelBase> m_NetworkChannels;

    /// <summary>
    /// 初始化网络管理器的新实例。
    /// </summary>
    public NetworkManager()
    {
        m_NetworkChannels = new Dictionary<string, NetworkChannelBase>(StringComparer.Ordinal);
    }

    /// <summary>
    /// 获取网络频道数量。
    /// </summary>
    public int NetworkChannelCount
    {
        get
        {
            return m_NetworkChannels.Count;
        }
    }


    /// <summary>
    /// 网络管理器轮询。
    /// </summary>
    /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
    /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
    internal override void Update(float elapseSeconds, float realElapseSeconds)
    {
        foreach (KeyValuePair<string, NetworkChannelBase> networkChannel in m_NetworkChannels)
        {
            networkChannel.Value.Update(elapseSeconds, realElapseSeconds);
        }
    }

    /// <summary>
    /// 关闭并清理网络管理器。
    /// </summary>
    internal override void Shutdown()
    {
        foreach (KeyValuePair<string, NetworkChannelBase> networkChannel in m_NetworkChannels)
        {
            NetworkChannelBase networkChannelBase = networkChannel.Value;
            networkChannelBase.NetworkChannelConnected -= OnNetworkChannelConnected;
            networkChannelBase.NetworkChannelClosed -= OnNetworkChannelClosed;
            networkChannelBase.NetworkChannelMissHeartBeat -= OnNetworkChannelMissHeartBeat;
            networkChannelBase.NetworkChannelError -= OnNetworkChannelError;
            networkChannelBase.NetworkChannelCustomError -= OnNetworkChannelCustomError;
            networkChannelBase.Shutdown();
        }

        m_NetworkChannels.Clear();
    }

    /// <summary>
    /// 检查是否存在网络频道。
    /// </summary>
    /// <param name="name">网络频道名称。</param>
    /// <returns>是否存在网络频道。</returns>
    public bool HasNetworkChannel(string name)
    {
        return m_NetworkChannels.ContainsKey(name ?? string.Empty);
    }

    /// <summary>
    /// 获取网络频道。
    /// </summary>
    /// <param name="name">网络频道名称。</param>
    /// <returns>要获取的网络频道。</returns>
    public INetworkChannel GetNetworkChannel(string name)
    {
        NetworkChannelBase networkChannel = null;
        if (m_NetworkChannels.TryGetValue(name ?? string.Empty, out networkChannel))
        {
            return networkChannel;
        }

        return null;
    }

    /// <summary>
    /// 获取所有网络频道。
    /// </summary>
    /// <returns>所有网络频道。</returns>
    public INetworkChannel[] GetAllNetworkChannels()
    {
        int index = 0;
        INetworkChannel[] results = new INetworkChannel[m_NetworkChannels.Count];
        foreach (KeyValuePair<string, NetworkChannelBase> networkChannel in m_NetworkChannels)
        {
            results[index++] = networkChannel.Value;
        }

        return results;
    }

    /// <summary>
    /// 获取所有网络频道。
    /// </summary>
    /// <param name="results">所有网络频道。</param>
    public void GetAllNetworkChannels(List<INetworkChannel> results)
    {
        if (results == null)
        {
            //todo: 这里需要抛出异常
            // throw new GameFrameworkException("Results is invalid.");
        }

        results.Clear();
        foreach (KeyValuePair<string, NetworkChannelBase> networkChannel in m_NetworkChannels)
        {
            results.Add(networkChannel.Value);
        }
    }

    /// <summary>
    /// 创建网络频道。
    /// </summary>
    /// <param name="name">网络频道名称。</param>
    /// <param name="EServiceType">网络服务类型。</param>
    /// <param name="networkChannelHelper">网络频道辅助器。</param>
    /// <returns>要创建的网络频道。</returns>
    public INetworkChannel CreateNetworkChannel(string name, EServiceType EServiceType, INetworkChannelHelper networkChannelHelper)
    {
        if (networkChannelHelper == null)
        {
            //todo: 这里需要抛出异常
            // throw new GameFrameworkException("Network channel helper is invalid.");
        }

        if (networkChannelHelper.PacketHeaderLength < 0)
        {
            //todo: 这里需要抛出异常
            // throw new GameFrameworkException("Packet header length is invalid.");
        }

        if (HasNetworkChannel(name))
        {
            //todo: 这里需要抛出异常
            // throw new GameFrameworkException(Utility.Text.Format("Already exist network channel '{0}'.", name ?? string.Empty));
        }

        NetworkChannelBase networkChannel = null;
        switch (EServiceType)
        {
            case EServiceType.Tcp:
                networkChannel = new TcpNetworkChannel(name, networkChannelHelper);
                break;

            case EServiceType.TcpWithSyncReceive:
                networkChannel = new TcpWithSyncReceiveNetworkChannel(name, networkChannelHelper);
                break;

            default:
                //todo: 这里需要抛出异常
                // throw new GameFrameworkException(Utility.Text.Format("Not supported service type '{0}'.", EServiceType.ToString()));
                break;
        }

        networkChannel.NetworkChannelConnected += OnNetworkChannelConnected;
        networkChannel.NetworkChannelClosed += OnNetworkChannelClosed;
        networkChannel.NetworkChannelMissHeartBeat += OnNetworkChannelMissHeartBeat;
        networkChannel.NetworkChannelError += OnNetworkChannelError;
        networkChannel.NetworkChannelCustomError += OnNetworkChannelCustomError;
        m_NetworkChannels.Add(name, networkChannel);
        return networkChannel;
    }

    /// <summary>
    /// 销毁网络频道。
    /// </summary>
    /// <param name="name">网络频道名称。</param>
    /// <returns>是否销毁网络频道成功。</returns>
    public bool DestroyNetworkChannel(string name)
    {
        NetworkChannelBase networkChannel = null;
        if (m_NetworkChannels.TryGetValue(name ?? string.Empty, out networkChannel))
        {
            networkChannel.NetworkChannelConnected -= OnNetworkChannelConnected;
            networkChannel.NetworkChannelClosed -= OnNetworkChannelClosed;
            networkChannel.NetworkChannelMissHeartBeat -= OnNetworkChannelMissHeartBeat;
            networkChannel.NetworkChannelError -= OnNetworkChannelError;
            networkChannel.NetworkChannelCustomError -= OnNetworkChannelCustomError;
            networkChannel.Shutdown();
            return m_NetworkChannels.Remove(name);
        }

        return false;
    }

    private void OnNetworkChannelConnected(NetworkChannelBase networkChannel, object userData)
    {
        //todo: 抛事件
    }

    private void OnNetworkChannelClosed(NetworkChannelBase networkChannel)
    {
        //todo: 抛事件
    }

    private void OnNetworkChannelMissHeartBeat(NetworkChannelBase networkChannel, int missHeartBeatCount)
    {
        //todo: 抛事件
    }

    private void OnNetworkChannelError(NetworkChannelBase networkChannel, NetworkErrorCode errorCode, SocketError socketErrorCode, string errorMessage)
    {
        //todo: 抛事件
    }

    private void OnNetworkChannelCustomError(NetworkChannelBase networkChannel, object customErrorData)
    {
        //todo: 抛事件
    }
}