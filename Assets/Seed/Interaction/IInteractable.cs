namespace Seed.Interaction
{
    public interface IInteractable<TActor>
    {
        void OnInteractEnter(TActor actor);
        void OnInteractStay(TActor actor);
        void OnInteractExit(TActor actor);
    }
}
