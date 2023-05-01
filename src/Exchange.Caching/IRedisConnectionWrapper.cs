using StackExchange.Redis;
using System;
using System.Net;

namespace Exchange.Caching
{
    public interface IRedisConnectionWrapper : IDisposable
    {
        IDatabase GetDatabase(int db);

        IServer GetServer(EndPoint endPoint);

        EndPoint[] GetEndPoints();

        void FlushDatabase(RedisDatabaseNumber db);

    }
}
