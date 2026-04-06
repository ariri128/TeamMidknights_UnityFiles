using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GuardAI : MonoBehaviour
{
    public Transform player;

    public Vector3 floorCenter;
    public float floorPatrolRadius = 8f;
    public float navMeshSearchDistance = 4f;

    public float patrolWaitTime = 1.5f;
    public float chaseRadius = 5f;
    public float attackRadius = 1.2f;
    public float losePlayerRadius = 7f;

    public int damageAmount = 50;
    public float damageInterval = 1f;

    private NavMeshAgent agent;
    private float waitTimer;
    private float damageTimer;
    private GuardState currentState;

    private float normalSpeed;
    private float normalAngularSpeed;
    private float normalAcceleration;

    private enum GuardState
    {
        Patrol,
        Chase,
        Attack
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        normalSpeed = agent.speed;
        normalAngularSpeed = agent.angularSpeed;
        normalAcceleration = agent.acceleration;
    }

    private void Start()
    {
        currentState = GuardState.Patrol;
        SetRandomPatrolPoint();
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case GuardState.Patrol:
                if (distanceToPlayer <= chaseRadius)
                {
                    currentState = GuardState.Chase;
                    break;
                }

                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    waitTimer += Time.deltaTime;

                    if (waitTimer >= patrolWaitTime)
                    {
                        waitTimer = 0f;
                        SetRandomPatrolPoint();
                    }
                }
                break;

            case GuardState.Chase:
                if (distanceToPlayer <= attackRadius)
                {
                    currentState = GuardState.Attack;
                    agent.ResetPath();
                    damageTimer = 0f;
                    break;
                }

                if (distanceToPlayer > losePlayerRadius)
                {
                    currentState = GuardState.Patrol;
                    SetRandomPatrolPoint();
                    break;
                }

                agent.SetDestination(player.position);
                break;

            case GuardState.Attack:
                Vector3 lookTarget = new Vector3(player.position.x, transform.position.y, player.position.z);
                transform.LookAt(lookTarget);

                if (distanceToPlayer > attackRadius)
                {
                    currentState = GuardState.Chase;
                    break;
                }

                damageTimer += Time.deltaTime;

                if (damageTimer >= damageInterval)
                {
                    damageTimer = 0f;

                    PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(damageAmount);
                    }
                }

                break;
        }
    }

    private void SetRandomPatrolPoint()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPoint = floorCenter + new Vector3(
                Random.Range(-floorPatrolRadius, floorPatrolRadius),
                0f,
                Random.Range(-floorPatrolRadius, floorPatrolRadius)
            );

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, navMeshSearchDistance, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                return;
            }
        }
    }

    public void ApplySlow(float speedMultiplier)
    {
        agent.speed = normalSpeed * speedMultiplier;
        agent.angularSpeed = normalAngularSpeed * speedMultiplier;
        agent.acceleration = normalAcceleration * speedMultiplier;
    }

    public void RestoreNormalSpeed()
    {
        agent.speed = normalSpeed;
        agent.angularSpeed = normalAngularSpeed;
        agent.acceleration = normalAcceleration;
    }
}
