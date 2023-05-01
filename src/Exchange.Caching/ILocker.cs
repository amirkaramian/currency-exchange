using System;

namespace Exchange.Caching
{
    public interface ILocker
    {
        bool PerformActionWithLock(string resource, TimeSpan expirationTime, Action action);
    }
}
