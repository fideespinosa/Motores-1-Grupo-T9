public class AttackState : EnemyState
{
    public AttackState(EnemyMovement enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.TriggerAttack(); // sonido, ResetPath, Die() -> minijuego
    }

    public override void Tick()
    {
        if (enemy.patrolDuringMinigame)
        {
            enemy.PatrolState.Patrol();
        }
    }
}