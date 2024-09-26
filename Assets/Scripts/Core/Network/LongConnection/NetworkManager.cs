using UnityEngine;
using System;
using System.IO;


public class NetworkManager
{
    public delegate void NetworkEventCallback(int errorCode = 0);
    public delegate void ReceiveEventCallback(MemoryStream pack);

    public event NetworkEventCallback connectFailEvent;
    public event NetworkEventCallback reconnectFailEvent;
    public event NetworkEventCallback connectCompleteEvent;
    public event NetworkEventCallback disconnectedCompleteEvent;
    public ReceiveEventCallback receiveEventCallback;

    private INetworkState networkState = null;
    private NetworkSyncQueue networkSyncQueue = null;
    private IClientSession clientSession = null;
    private IProtocolResolver protocolResolver = null;
    private string ip;
    private int port;

    private TCPCommon.NETWORK_STATE tcpState;
    public TCPCommon.NETWORK_STATE TcpState
    {
        get { return tcpState; }
    }

    /// <summary>
    /// 连接设置
    /// </summary>
    /// <param name="host"></param>
    /// <param name="port"></param>
    /// <param name="protocolResolver"></param>
    public void SettingConnector(string host, int port, IProtocolResolver protocolResolver)
    {
        this.ip = host;
        this.port = port;
        this.protocolResolver = protocolResolver;
    }

    /// <summary>
    /// 连接
    /// </summary>
    /// <param name="host"></param>
    /// <param name="port"></param>
    /// <param name="protocolResolver"></param>
    public void Connect(string host, int port, IProtocolResolver protocolResolver)
    {
        SwitchStateHandle(TCPCommon.NETWORK_STATE.NONE);
        networkState.Connect(host, port, protocolResolver);
    }

    /// <summary>
    /// 断开连接
    /// </summary>
    public void Disconnect()
    {
        Debug.Log("网络断开 :" + networkState);
        networkState.Disconnect();
    }

    /// <summary>
    /// 重连
    /// </summary>
    public void Reconnect()
    {
        if (tcpState == TCPCommon.NETWORK_STATE.CONNECT)
        {
            Disconnect();
            Update();
        }
        Debug.Log("网络重连 :" + networkState);
        networkState.Reconnect();
    }

    /// <summary>
    /// 切换网络状态
    /// </summary>
    /// <param name="state"></param>
    public void SwitchStateHandle(TCPCommon.NETWORK_STATE state)
    {
        if (tcpState == state)
            return;


        Debug.Log("切换网络处理方式:" + state);

        tcpState = state;
        switch (tcpState)
        {
            case TCPCommon.NETWORK_STATE.CONNECT:
                {
                    networkState = new NetworkConnectedState(ip, port, protocolResolver, clientSession);
                }
                break;

            case TCPCommon.NETWORK_STATE.DISCONNECT:
                {
                    networkState = new NetworkDisconnectedState(ip, port, protocolResolver, clientSession);
                }
                break;

            case TCPCommon.NETWORK_STATE.NONE:
                {
                    networkState = new NetworkNoneState(ip, port, protocolResolver, clientSession);
                }
                break;

            default:
                return;

        }

        networkState.Enter(this);
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="packet"></param>
    public void Send(MemoryStream packet)
    {
        networkState.Send(packet);
    }

    private void OnReceive(MemoryStream packet)
    {
        receiveEventCallback(packet);
    }

    public void Update()
    {
        if (networkSyncQueue == null)
        {
            return;
        }
        if (this.networkSyncQueue.HasNetworkStateEvent())
        {
            Const<TCPCommon.NETWORK_STATE> constNetworkState = this.networkSyncQueue.PopNetworkStateEvent();
            if (constNetworkState != null)
            {
                SwitchStateHandle(constNetworkState.Value);
            }
        }
        //网络断掉的时候最后一个包不会收到
        int prcess = 0;
        while (this.networkSyncQueue.HasReceivePacket())
        {
            MemoryStream packet = this.networkSyncQueue.PopReceivePacket();
            if (packet != null)
            {
                OnReceive(packet);
            }
            prcess++;
            if (prcess == TCPCommon.PACKET_PROCESS_PERFRAME)
            {
                break;
            }
        }

        clientSession?.Update();
    }

    public void OnConnnectComplete(TCPSession tcpSession)
    {
        networkSyncQueue = new NetworkSyncQueue();
        clientSession = new ClientSession(tcpSession, networkSyncQueue, protocolResolver);
    }
    public void OnConnectFail(int errorCode)
    {
        if (connectFailEvent != null)
        {
            connectFailEvent(errorCode);
        }
    }
    public void OnReconnectComplete(TCPSession tcpSession)
    {

    }
    public void OnReconnectFail(int errorCode)
    {
        if (reconnectFailEvent != null)
        {
            reconnectFailEvent(errorCode);
        }
    }
    public void OnConnectEventFire()
    {
        if (connectCompleteEvent != null)
            connectCompleteEvent();
    }
    public void OnDisconnectEventFire()
    {
        if (disconnectedCompleteEvent != null)
            disconnectedCompleteEvent();
    }

}