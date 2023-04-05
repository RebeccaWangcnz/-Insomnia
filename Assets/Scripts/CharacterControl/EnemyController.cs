using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public EnemyState state;

    [Header("Move")]
    public float idleSpeed = 200.0f;
    public float chaseSpeed = 300.0f;
    public float nextWaypointDistance = 3f;
    [Header("Patrol")]
    public Transform[] patrolPoint;
    public float searchRadius;

    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;

    private Seeker seeker;
    private Rigidbody2D rb;
    private Transform target;
    private float speed;

    private int idlePatrolIndex;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        speed = idleSpeed;

        InvokeRepeating("UpdataPath", 0f, .5f);
    }

    private void FixedUpdate()
    {
        switch(state)
        {
            case EnemyState.Idle:
                IdleFaceDirection();
                IdlePatrol();
                break;
            case EnemyState.Chase:
                ChaseFaceDirection();
                ChasePatrol();
                break;
        }
        MoveToTarget();
    }
    private void IdleFaceDirection()
    {
        float direction = -Mathf.Sign(rb.velocity.x);
        transform.GetChild(0).localScale = new Vector3(direction, 1, 1);
    }
    private void ChaseFaceDirection()
    {
        float direction = -Mathf.Sign(rb.velocity.x- player.position.x );
        transform.GetChild(0).localScale = new Vector3(direction, 1, 1);
    }
    private void ChasePatrol()
    {
        float distancePlayer = Vector2.Distance(rb.position, player.position);
        if (distancePlayer > searchRadius)
        {
            target = patrolPoint[0];
            state = EnemyState.Idle;
            speed = idleSpeed;
        }
        MoveToTarget();
    }
    private void IdlePatrol()
    {
        float distance = Vector2.Distance(rb.position, patrolPoint[idlePatrolIndex].position);
        if (distance<1)
        {
            idlePatrolIndex = (idlePatrolIndex + 1) % patrolPoint.Length;
        }
        target = patrolPoint[idlePatrolIndex];
        float distancePlayer = Vector2.Distance(rb.position, player.position);
        if(distancePlayer < searchRadius)
        {
            target = player;
            state = EnemyState.Chase;
            speed = chaseSpeed;
        }
        MoveToTarget();
    }
    private void MoveToTarget()
    {
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        //enemy move
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;
        rb.velocity = new Vector2(force.x, 0);//only move on the platform

        //change point
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }
    private void UpdataPath()
    {
        if (seeker.IsDone())
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }
    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
}
