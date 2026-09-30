public class ChaseState : EnemyState
{
    public ChaseState(EnemyMovement enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.OnChaseStarted(); // rugido + musica de combate
    }

    public override void Tick()
    {
        // Perdio al jugador -> vuelve a patrullar
        if (!enemy.ShouldChase())
        {
            enemy.OnChaseEnded(); // apaga musica de combate
            enemy.ChangeState(enemy.PatrolState);
            return;
        }

        // Lo alcanzo -> ataca
        if (enemy.DistanceToPlayer() <= enemy.attackRange && !enemy.IsAttacking)
        {
            enemy.ChangeState(enemy.AttackState);
            return;
        }

        if (!enemy.IsAttacking && !enemy.MinigameActive)
        {
            enemy.MoveTowards(enemy.Player.position, enemy.chaseSpeed);
        }
    }
}