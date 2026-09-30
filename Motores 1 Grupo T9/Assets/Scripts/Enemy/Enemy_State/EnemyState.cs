public abstract class EnemyState
{
    protected readonly EnemyMovement enemy;

    protected EnemyState(EnemyMovement enemy)
    {
        this.enemy = enemy;
    }

    public virtual void Enter() { }
    public abstract void Tick();
    public virtual void Exit() { }
}

public class EnemyStateMachine
{
    public EnemyState Current { get; private set; }

    public void ChangeState(EnemyState next)
    {
        if (next == null || next == Current) return;

        Current?.Exit();
        Current = next;
        Current.Enter();
    }

    public void Tick()
    {
        Current?.Tick();
    }
}