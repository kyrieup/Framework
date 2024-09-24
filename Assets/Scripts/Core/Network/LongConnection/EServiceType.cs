/// <summary>
/// 网络服务类型。
/// </summary>
public enum EServiceType : byte
{
    /// <summary>
    /// TCP 网络服务。
    /// </summary>
    Tcp = 0,

    /// <summary>
    /// 使用同步接收的 TCP 网络服务。
    /// </summary>
    TcpWithSyncReceive
}