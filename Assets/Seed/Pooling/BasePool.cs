using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Seed.Pooling
{
    public abstract class BasePool<TKey, TItem> : MonoBehaviour
        where TKey : Enum
        where TItem : Component, IPoolable<TKey>
    {
        [Serializable]
        protected struct PoolEntry
        {
            public TKey key;
            public TItem prefab;
            public int defaultCapacity;
            public int maxSize;
        }

        [SerializeField] private PoolEntry[] entries;

        private readonly Dictionary<TKey, IObjectPool<TItem>> _pools = new();

        protected Dictionary<TKey, IObjectPool<TItem>> Pools => _pools;

        protected virtual void Awake()
        {
            if (entries is null) return;

            foreach (PoolEntry entry in entries)
            {
                if (entry.prefab is null) continue;

                TItem prefab = entry.prefab;
                _pools[entry.key] = new ObjectPool<TItem>(
                    createFunc: () => CreateItem(prefab),
                    actionOnGet: OnGet,
                    actionOnRelease: OnRelease,
                    actionOnDestroy: OnDestroyItem,
                    defaultCapacity: Mathf.Max(0, entry.defaultCapacity),
                    maxSize: Mathf.Max(1, entry.maxSize)
                );
            }
        }

        protected virtual TItem CreateItem(TItem prefab) => Instantiate(prefab, transform);

        protected virtual void OnGet(TItem item)
        {
            item.gameObject.SetActive(true);
        }

        protected virtual void OnRelease(TItem item)
        {
            item.transform.SetParent(transform);
            item.gameObject.SetActive(false);
        }

        protected virtual void OnDestroyItem(TItem item) => Destroy(item.gameObject);

        public TItem Spawn(TKey key, Vector3 position, Quaternion rotation)
            => PoolOps.Spawn(_pools, key, position, rotation);

        public TItem Spawn(TKey key, Vector3 position) => Spawn(key, position, Quaternion.identity);

        public void Despawn(TItem item) => PoolOps.Despawn(_pools, item);
    }
}
