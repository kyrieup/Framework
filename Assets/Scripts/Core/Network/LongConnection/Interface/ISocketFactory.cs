using System;
using System.Net;
public interface ISocketFactory
{
    ISocket Create(string protocol, IPEndPoint ipEndPoint = null);

    ISocket Create(string protocol, Uri uri = null);

    void Extend(string protocol, Func<object, ISocket> maker);

}