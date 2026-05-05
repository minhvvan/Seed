using System;
using Seed.CameraSystem;
using UnityEngine;

namespace Seed.Audio
{
    public static class SoundUtil<TId> where TId : Enum
    {
        private static SoundLibrarySO<TId> _library;

        public static void Initialize(SoundLibrarySO<TId> library) => _library = library;

        public static void Play(TId id, AudioSource source)
        {
            if (_library is null || source is null) return;
            if (!source.gameObject.IsOnScreen()) return;
            if (!_library.TryGet(id, out SoundLibrarySO<TId>.SoundEntry entry)) return;
            if (entry.clip is null) return;

            float vol = entry.volume <= 0f ? 1f : entry.volume;
            source.PlayOneShot(entry.clip, vol * _library.MasterVolume);
        }
    }
}
