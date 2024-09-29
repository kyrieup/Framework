using System;
/// <summary>
/// 消息编解码器
/// </summary>
public interface IMsgCode
{
    bool Input(byte[] source, int offset, int count, out ReadOnlySpan<byte> result, out Exception ex);

    void Reset();

    ReadOnlySpan<byte> Pack(object packet);

    object Unpack(ReadOnlySpan<byte> rawData);
}