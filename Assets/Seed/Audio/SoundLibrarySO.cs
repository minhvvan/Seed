using System;
using System.Collections.Generic;
using UnityEngine;

namespace Seed.Audio
{
    public abstract class SoundLibrarySO<TId> : ScriptableObject where TId : Enum
    {
        [Serializable]
        public struct SoundEntry
        {
            public TId id;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume;
        }

        [SerializeField] private SoundEntry[] entries;
        [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;

        private Dictionary<TId, SoundEntry> _lookup;

        public float MasterVolume => masterVolume;

        public bool TryGet(TId id, out SoundEntry entry)
        {
            BuildIfNeeded();
            return _lookup.TryGetValue(id, out entry);
        }

        private void OnEnable() => _lookup = null;

        private void BuildIfNeeded()
        {
            if (_lookup is not null) return;
            _lookup = new Dictionary<TId, SoundEntry>();
            if (entries is null) return;
            foreach (SoundEntry entry in entries)
            {
                if (entry.clip is null) continue;
                _lookup[entry.id] = entry;
            }
        }
    }
}
