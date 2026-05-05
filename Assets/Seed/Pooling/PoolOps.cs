using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Seed.Pooling
{
    public static class PoolOps
    {
        public static TItem Spawn<TKey, TItem>(
            Dictionary<TKey, IObjectPool<TItem>> pools,
            TKey key,
            Vector3 position,
            Quaternion rotation)
            where TItem : Component, IPoolable<TKey>
            where TKey : Enum
        {
            if (pools is null || !pools.TryGetValue(key, out IObjectPool<TItem> pool)) return null;

            TItem item = pool.Get();
            item.transform.SetPositionAndRotation(position, rotation);
            return item;
        }

        public static void Despawn<TKey, TItem>(
            Dictionary<TKey, IObjectPool<TItem>> pools,
            TItem item)
            where TItem : Component, IPoolable<TKey>
            where TKey : Enum
        {
            if (item is null) return;

            if (pools is null || !pools.TryGetValue(item.PoolKey, out IObjectPool<TItem> pool))
            {
                UnityEngine.Object.Destroy(item.gameObject);
                return;
            }

            pool.Release(item);
        }
    }
}
