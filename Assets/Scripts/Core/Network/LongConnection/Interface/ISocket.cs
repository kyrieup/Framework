using System;
using System.Net;
using System.Threading.Tasks;
public interface ISocket : IDisposable
{

    /// <summary>
    /// 名称
    /// </summary>
    string Name
    {
        get;
    }

    /// <summary>
    /// 是否连接
    /// </summary>
    bool Connected
    {
        get;
    }

    /// <summary>
    /// 发送字节数
    /// </summary>

    long SentBytes
    {
        get;
    }

    /// <summary>
    /// 接收字节数
    /// </summary>
    long ReceiveBytes
    {
        get;
    }

    /// <summary>
    /// 关闭
    /// </summary>
    /// <returns></returns>
    event Action<ISocket, Exception> OnClosed;

    /// <summary>
    /// 消息
    /// </summary>
    event Action<ISocket, ArraySegment<byte>> OnMessage;

    /// <summary>
    /// 错误
    /// </summary>
    event Action<ISocket, Exception> OnError;

    /// <summary>
    /// 改变IP
    /// </summary>
    /// <param name="ipEndPoint"></param>
    void ChangeIpEndPoint(IPEndPoint ipEndPoint);

    /// <summary>
    /// 改变URI
    /// </summary>
    void ChangeUri(Uri uri);

    /// <summary>
    /// 连接
    /// </summary>
    /// <returns></returns>
    Task<SocketConnectResult> Connect();

    /// <summary>
    /// 发送
    /// </summary>
    SendResults Send(byte[] data);

    /// <summary>
    /// 发送
    /// </summary>
    /// <param name="data"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    SendResults Send(byte[] data, int offset, int count);

    /// <summary>
    /// 发送
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    SendResults Send(ReadOnlySpan<byte> data);

    /// <summary>
    /// 关闭
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    CloseResults Disconnect(Exception exception = null);

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="deltaTime"></param>
    /// <param name="unsacaleTime"></param>
    void OnUpdate(float deltaTime, float unsacaleTime);
}

public struct SocketConnectResult
{
    /// <summary>
    /// 结果
    /// </summary>
    public ConnectResults Result { internal set; get; }

    /// <summary>
    /// 异常
    /// </summary>
    public Exception Exception { internal set; get; }
}