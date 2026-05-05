using UnityEngine;

namespace Seed.Interaction
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public abstract class ActorInteractor<TActor> : MonoBehaviour where TActor : Component
    {
        [SerializeField] protected TActor actor;

        private IInteractable<TActor> _currentInteractable;

        protected virtual void Reset()
        {
            actor = GetComponentInParent<TActor>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_currentInteractable is Object obj && obj == null) _currentInteractable = null;
            if (_currentInteractable is not null) return;
            if (!other.TryGetComponent(out IInteractable<TActor> interactable)) return;

            _currentInteractable = interactable;
            interactable.OnInteractEnter(actor);
        }

        private void OnTriggerStay(Collider other)
        {
            if (_currentInteractable is null) return;
            if (!other.TryGetComponent(out IInteractable<TActor> interactable)) return;
            if (!ReferenceEquals(interactable, _currentInteractable)) return;

            interactable.OnInteractStay(actor);
        }

        private void OnTriggerExit(Collider other)
        {
            if (_currentInteractable is null) return;
            if (!other.TryGetComponent(out IInteractable<TActor> interactable)) return;
            if (!ReferenceEquals(interactable, _currentInteractable)) return;

            interactable.OnInteractExit(actor);
            _currentInteractable = null;
        }
    }
}
