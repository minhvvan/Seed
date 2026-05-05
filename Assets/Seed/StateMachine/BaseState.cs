namespace Seed.StateMachine
{
    public abstract class BaseState<TContext> : IState
    {
        protected readonly TContext context;
        protected readonly StateMachine<TContext> stateMachine;

        protected BaseState(TContext context, StateMachine<TContext> stateMachine)
        {
            this.context = context;
            this.stateMachine = stateMachine;
        }

        public virtual void Enter() { }
        public virtual void Tick() { }
        public virtual void Exit() { }
    }
}
