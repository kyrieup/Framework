using System;
using System.IO;
using UnityEngine;

class NetworkNoneState : INetworkState
    {
        private NetworkManager mNetworkManager;
        private String mHost;
        private int mPort;
        private IProtocolResolver mProcotolResolver = null;
        private IClientSession mClientSession = null;

        public NetworkNoneState(String host, int port, IProtocolResolver protocolResolver, IClientSession clientSession)
        {
            this.mHost = host;
            this.mPort = port;
            this.mProcotolResolver = protocolResolver;
            this.mClientSession = clientSession;
        }

        public void Enter(NetworkManager networkManager)
        {
            this.mNetworkManager = networkManager;
        }

        public void Connect(string host, int port, IProtocolResolver protocolResolver)
        {
            TCPConnector connector = new TCPConnector();
            connector.connectHandler = mNetworkManager.OnConnnectComplete;
            connector.failHandler = mNetworkManager.OnConnectFail;
            connector.Connect(host, port, protocolResolver);
        }

        public void Disconnect()
        {

        }

        public void Reconnect()
        {
            TCPConnector connector = new TCPConnector();
            connector.connectHandler = mNetworkManager.OnReconnectComplete;
            connector.failHandler = mNetworkManager.OnReconnectFail;
            connector.Connect(mHost, mPort, mProcotolResolver, mClientSession.TcpSession);
        }

        public void Send(MemoryStream packet)
        {
            Debug.Log("断网了不能发包");
        }


    }