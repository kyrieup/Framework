using System;
using System.Net;
using System.Net.Sockets;

public class TcpConnector : SocketConnector
{
    public TcpConnector(string name, IPEndPoint ipEndPoint)
    : base(name, ipEndPoint, ProtocolType.Tcp)
    {

    }

    protected override System.Net.Sockets.Socket MakeSocket()
    {
        System.Net.Sockets.Socket socket = base.MakeSocket();
        socket.NoDelay = true;
        //socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, optionValue: true);
        return socket;
    }
}

