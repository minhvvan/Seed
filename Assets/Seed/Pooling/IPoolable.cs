using System;

namespace Seed.Pooling
{
    public interface IPoolable<TKey> where TKey : Enum
    {
        TKey PoolKey { get; }
    }
}
