public class AttackState : EnemyState
{
    public AttackState(EnemyMovement enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.PatrolState.SkipToNextWaypoint(); // sigue hacia el proximo waypoint
        enemy.TriggerAttack();                  // sonido + contador + minijuego
        enemy.ChangeState(enemy.PatrolState);   // vuelve a patrullar ya
    }

    public override void Tick() { }
}