namespace Seed.StateMachine
{
    public class StateMachine<TContext>
    {
        public IState Current { get; private set; }

        public void ChangeState(IState newState)
        {
            if (newState is null) return;
            if (ReferenceEquals(newState, Current)) return;

            Current?.Exit();
            Current = newState;
            Current.Enter();
        }

        public void Tick()
        {
            Current?.Tick();
        }
    }
}
