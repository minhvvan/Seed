using System;
using UnityEngine;

namespace Seed.Audio
{
    public abstract class SoundLibraryBootstrap<TId> : MonoBehaviour where TId : Enum
    {
        [SerializeField] private SoundLibrarySO<TId> library;

        protected virtual void Awake() => SoundUtil<TId>.Initialize(library);
    }
}
