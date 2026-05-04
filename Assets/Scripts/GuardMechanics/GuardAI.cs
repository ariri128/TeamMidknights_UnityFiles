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
    private GuardAnimationController guardAnim;
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
        guardAnim = GetComponent<GuardAnimationController>();

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
                guardAnim?.SetWalking(true);
                guardAnim?.SetAttacking(false);

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
                guardAnim?.SetWalking(true);
                guardAnim?.SetAttacking(false);

                if (distanceToPlayer <= attackRadius)
                {
                    currentState = GuardState.Attack;
                    agent.ResetPath();

                    DealDamageToPlayer();
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
                guardAnim?.SetAttacking(true);
                guardAnim?.SetWalking(false);

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
                    DealDamageToPlayer();
                }

                break;
        }
    }

    private void DealDamageToPlayer()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
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

    // Forces the guard immediately into Chase state — used only by the tutorial.
    public void ForceChase()
    {
        currentState = GuardState.Chase;
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

    /*
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
    private GuardAnimationController guardAnim;
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
        guardAnim = GetComponent<GuardAnimationController>();

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
                guardAnim?.SetWalking(true);
                guardAnim?.SetAttacking(false);

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
                guardAnim?.SetWalking(true);
                guardAnim?.SetAttacking(false);

                if (distanceToPlayer <= attackRadius)
                {
                    currentState = GuardState.Attack;
                    agent.ResetPath();

                    DealDamageToPlayer();
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
                guardAnim?.SetAttacking(true);
                guardAnim?.SetWalking(false);

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
                    DealDamageToPlayer();
                }

                break;
        }
    }

    private void DealDamageToPlayer()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
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
    */
}
