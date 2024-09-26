using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine;


public class TCPSession
{
    public enum SESSION_STATE
    {
        NONE,
        CONNECTED,
        DISCONNECTED,
    }

    private Socket mSocket;
    private SocketAsyncEventArgs mReceiveEventArgs;
    private SocketAsyncEventArgs mSendEventArgs;

    private CircularBuffer mReceiveBuffer = new CircularBuffer(TCPCommon.RECV_BUFFERQUEUE_POWER);
    private CircularBuffer mSendBuffer = new CircularBuffer(TCPCommon.SEND_BUFFERQUEUE_POWER);
    private byte[] mReadQueueBuffer = new byte[TCPCommon.RECV_BUFFER_SIZE + TCPCommon.PACKET_BUFFER_SIZE];

    private IProtocolResolver mProtocolResolver;
    private WeakReference mClientSession;

    private SESSION_STATE mSessionState;
    public SESSION_STATE sessionState
    {
        get { return mSessionState; }
    }

    private object mSendingBufferLock;

    public void SetClientSession(IClientSession client_session)
    {
        this.mClientSession = new WeakReference(client_session, false);
    }

    public void SetProtocolResolver(IProtocolResolver protocolResolver)
    {
        this.mProtocolResolver = protocolResolver;
    }

    private StreamWriter _streamWriter;

    public TCPSession()
    {
        this.mSendingBufferLock = new object();
        mSessionState = SESSION_STATE.NONE;
    }

    public void ReceiveRequest()
    {
        if (mSessionState != SESSION_STATE.CONNECTED)
            return;

        bool pending = mSocket.ReceiveAsync(mReceiveEventArgs);
        if (false == pending)
        {
            OnReceive();
        }
    }

    public void SendRequest(MemoryStream packet)
    {
        if (mSessionState != SESSION_STATE.CONNECTED)
            return;

        if (packet == null)
            return;

        if (packet == null || packet.Length <= 0)
            return;

        lock (mSendingBufferLock)
        {
            int stored_size = mSendBuffer.GetStoredSize();
            if (false == mSendBuffer.Write(packet.ToArray(), 0, (int)packet.Length))
            {
            }

            if (stored_size <= 0)
            {
                SendFlush();
            }


        }
    }

    private void SendFlush()
    {
        if (mSessionState != SESSION_STATE.CONNECTED)
            return;

        int storedSize = 0;
        lock (mSendingBufferLock)
        {
            storedSize = mSendBuffer.GetStoredSize();

            mSendBuffer.Peek(mSendEventArgs.Buffer, storedSize);
        }

        mSendEventArgs.SetBuffer(mSendEventArgs.Offset, storedSize);
        bool pending = mSocket.SendAsync(mSendEventArgs);
        if (false == pending)
        {
            OnSend();
        }
    }

    public void OnConnect()
    {
        mSessionState = SESSION_STATE.CONNECTED;

        if (mClientSession.IsAlive)
        {
            (mClientSession.Target as IClientSession).OnConnected();
        }
        mSocket.Blocking = false;
    }


    public void OnReceive()
    {
        if (mReceiveEventArgs.BytesTransferred > 0 && mReceiveEventArgs.SocketError == SocketError.Success)
        {
            OnProcessReceive(mReceiveEventArgs.Buffer, mReceiveEventArgs.Offset, mReceiveEventArgs.BytesTransferred);

            ReceiveRequest();
        }
        else
        {
            OnDisconnect();
        }
    }

    public void OnSend()
    {
        if (mSendEventArgs.BytesTransferred <= 0 || mSendEventArgs.SocketError != SocketError.Success)
        {
            return;
        }

        int remainBufferSize = 0;
        lock (mSendingBufferLock)
        {
            mSendBuffer.Remove(mSendEventArgs.BytesTransferred);
            remainBufferSize = mSendBuffer.GetStoredSize();
        }

        if (remainBufferSize > 0)
        {
            SendFlush();
        }

    }

    public void OnDisconnect()
    {
        mSessionState = SESSION_STATE.DISCONNECTED;
        Reset();

        if (mClientSession.IsAlive)
        {
            (mClientSession.Target as IClientSession).OnDisconnected();
        }
    }


    private static string ToHexString(byte[] bytes) // 0xae00cf => "AE00CF "
    {
        string hexString = string.Empty;
        if (bytes != null)
        {
            StringBuilder strB = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
            {
                strB.Append(bytes[i].ToString("X2"));
                strB.Append(" ");
            }

            hexString = strB.ToString();
        }

        return hexString;
    }


    private void OnProcessReceive(byte[] buffer, int offset, int bytes)
    {
        if (false == mReceiveBuffer.Write(buffer, offset, bytes))
        {
            Disconnect();
        }

        int storedSize = mReceiveBuffer.GetStoredSize();
        int offsetIndex = 0;

        //            Debug.Log(offset+" 网络解包 OnProcessReceive storedSize："+storedSize);
        if (false == mReceiveBuffer.Peek(mReadQueueBuffer, storedSize))
        {

        }

        MemoryStream packet = null;
        int packetSize = 0;
        while ((packet = mProtocolResolver.PacketProtocolResolve(new ArraySegment<byte>(mReadQueueBuffer, offsetIndex, storedSize), out packetSize)) != null)
        {
            //Debug.Log(storedSize+" 网络解包队列 "+ToHexString(mReadQueueBuffer));
            if (false == mClientSession.IsAlive)
            {
                break;
            }
            (mClientSession.Target as IClientSession).OnRead(packet);

            offsetIndex += packetSize;
            storedSize -= packetSize;

            mReceiveBuffer.Remove(packetSize);

            //Debug.Log(offsetIndex+"  网络解包 storedSize："+storedSize+"  "+packetSize);

            if (storedSize <= 0)
            {
                //Debug.Log("网络解包");
                break;
            }
        }

    }

    public void Reset()
    {
        Debug.Log("网络数据重置");
        mSendBuffer.Clear();
        mReceiveBuffer.Clear();
    }

    public void Disconnect()
    {
        mSessionState = SESSION_STATE.DISCONNECTED;
        try
        {
            mSocket.Shutdown(SocketShutdown.Send);
        }
        catch (Exception) { }
        mSocket.Close();
        mSocket = null;

        //> 临时添加
        OnDisconnect();
    }

    public void Update()
    {
        return;
    }

    public void OnConnectCompleted(Socket socket)
    {
        if (sessionState == SESSION_STATE.NONE)
        {
            mReceiveEventArgs = new SocketAsyncEventArgs();
            mReceiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceiveCompleted);
            mReceiveEventArgs.UserToken = this;
            mReceiveEventArgs.SetBuffer(new byte[TCPCommon.RECV_BUFFER_SIZE], 0, TCPCommon.RECV_BUFFER_SIZE);

            mSendEventArgs = new SocketAsyncEventArgs();
            mSendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
            mSendEventArgs.UserToken = this;
            mSendEventArgs.SetBuffer(new byte[TCPCommon.SEND_BUFFER_SIZE], 0, TCPCommon.SEND_BUFFER_SIZE);
        }

        mSocket = socket;
    }

    public void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
    {
        TCPSession tcpSession = e.UserToken as TCPSession;
        if (e.LastOperation == SocketAsyncOperation.Receive)
        {
            tcpSession.OnReceive();
            return;
        }
        else
        {
            tcpSession.OnDisconnect();
        }
    }

    public void OnSendCompleted(object sender, SocketAsyncEventArgs e)
    {
        TCPSession tcpSession = e.UserToken as TCPSession;
        tcpSession.OnSend();
    }
}


