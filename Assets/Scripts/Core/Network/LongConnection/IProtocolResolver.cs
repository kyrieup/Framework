using System;
using System.IO;
public interface IProtocolResolver
{
    /// <summary>
    /// 数据包解析
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="bytes"></param>
    /// <param name="packet_size"></param>
    /// <returns></returns>
    MemoryStream PacketProtocolResolve(ArraySegment<byte> segmentBytes, out int packet_size);
}