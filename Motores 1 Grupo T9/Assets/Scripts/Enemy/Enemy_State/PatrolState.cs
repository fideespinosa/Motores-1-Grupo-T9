using UnityEngine;

public class PatrolState : EnemyState
{
    private int nowWaypoint = 0;
    private int waypointDirection = 1; // 1 = avanzando, -1 = retrocediendo (ping-pong)
    private float waitTimer = 0f;

    public PatrolState(EnemyMovement enemy) : base(enemy) { }

    public override void Tick()
    {
        if (enemy.ShouldChase())
        {
            enemy.ChangeState(enemy.ChaseState);
            return;
        }

        Patrol();
    }

    public void ResetPatrol()
    {
        nowWaypoint = 0;
        waypointDirection = 1;
        waitTimer = 0f;
    }

    // Logica de patrulla original (publica para reutilizarla desde otros estados)
    public void Patrol()
    {
        Transform[] waypoints = enemy.waypoints;

        if (waypoints.Length == 0)
            return;

        Transform wp = waypoints[nowWaypoint];

        float dist = enemy.FlatDistance(enemy.transform.position, wp.position);

        bool isCorner = (nowWaypoint == 0 || nowWaypoint == waypoints.Length - 1);

        if (dist <= enemy.waypointTolerance)
        {
            if (!isCorner)
            {
                AdvanceWaypointPingPong();
            }
            else if (waitTimer <= 0f)
            {
                waitTimer = enemy.waitAtWaypoint;

                if (enemy.Agent != null)
                {
                    enemy.Agent.ResetPath();
                }
            }
            else
            {
                waitTimer -= Time.deltaTime;

                if (waitTimer <= 0f)
                {
                    AdvanceWaypointPingPong();
                }
            }
        }
        else
        {
            enemy.MoveTowards(wp.position);
        }
    }

    void AdvanceWaypointPingPong()
    {
        Transform[] waypoints = enemy.waypoints;

        if (waypoints.Length <= 1)
            return;

        // Si estamos en un extremo, invertimos la direccion antes de avanzar
        if (nowWaypoint == waypoints.Length - 1)
        {
            waypointDirection = -1;
        }
        else if (nowWaypoint == 0)
        {
            waypointDirection = 1;
        }

        nowWaypoint += waypointDirection;
    }
    public int CurrentIndex => nowWaypoint;

    public void SkipToNextWaypoint()
    {
        waitTimer = 0f;
        AdvanceWaypointPingPong();
    }
}