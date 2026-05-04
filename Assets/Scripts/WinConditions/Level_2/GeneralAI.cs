using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class GeneralAI : MonoBehaviour
{
    [Header("Player Reference")]
    public Transform player;

    [Header("Patrol")]
    [Tooltip("The two points the General paces between before the player enters the room.")]
    public Transform patrolPointA;
    public Transform patrolPointB;

    [Tooltip("How close the General needs to get to a patrol point before turning around.")]
    public float patrolReachDistance = 0.5f;

    [Header("Detection")]
    [Tooltip("The trigger collider on the room. When the player enters it, the General starts attacking.")]
    public RoomTrigger roomTrigger;

    [Tooltip("How close the player needs to be to trigger the chase (once the room is entered).")]
    public float chaseRadius = 30f;

    [Tooltip("How far the player can get before the General gives up and returns to patrolling.")]
    public float leashRadius = 15f;

    [Header("Attack")]
    [Tooltip("Distance at which the General stops and deals damage. Larger than guards so he doesn't crowd the player.")]
    public float attackRadius = 3.0f;

    [Tooltip("Damage dealt to the player on each attack.")]
    public int damageAmount = 35;

    [Tooltip("Seconds between each attack.")]
    public float attackCooldown = 1.2f;

    [Header("Death")]
    [Tooltip("Seconds after falling before decision panel shows.")]
    public float decisionTriggerDelay = 3f;

    [Tooltip("Seconds after death before player movement stops and decision panel delay begins.")]
    public float playerStopDelay = 0.5f;

    private NavMeshAgent agent;
    private Rigidbody rb;
    private GeneralAnimationController generalAnim;

    private enum GeneralState { Patrol, Chase, Dead }
    private GeneralState currentState = GeneralState.Patrol;

    private Transform currentPatrolTarget;
    private float attackTimer = 0f;
    private bool isKnockedBack = false;

    private float normalSpeed;
    private float normalAngularSpeed;
    private float normalAcceleration;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        // Keeps Rigidbody from fighting NavMeshAgent during normal movement
        rb.isKinematic = true;
        generalAnim = GetComponent<GeneralAnimationController>();

        normalSpeed = agent.speed;
        normalAngularSpeed = agent.angularSpeed;
        normalAcceleration = agent.acceleration;

        generalAnim = GetComponent<GeneralAnimationController>();
        Debug.Log("GeneralAnim found: " + (generalAnim != null));
    }

    private void Start()
    {
        currentPatrolTarget = patrolPointA;
        agent.SetDestination(patrolPointA.position);
    }

    private void Update()
    {
        if (currentState == GeneralState.Dead) return;
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case GeneralState.Patrol:
                HandlePatrol();
                break;

            case GeneralState.Chase:
                HandleChase(distanceToPlayer);
                break;

        }
    }

    private void HandlePatrol()
    {
        if (currentPatrolTarget == null) return;

        generalAnim?.SetWalking(true);
        generalAnim?.SetAttacking(false);

        // If the player comes back within range, resume chasing
        if (player != null && Vector3.Distance(transform.position, player.position) <= chaseRadius)
        {
            currentState = GeneralState.Chase;
            return;
        }

        agent.SetDestination(currentPatrolTarget.position);

        float dist = Vector3.Distance(transform.position, currentPatrolTarget.position);
        if (dist <= patrolReachDistance)
        {
            // Swap patrol target
            currentPatrolTarget = (currentPatrolTarget == patrolPointA) ? patrolPointB : patrolPointA;
        }
    }

    private void HandleChase(float distanceToPlayer)
    {
        if (distanceToPlayer > leashRadius)
        {
            currentState = GeneralState.Patrol;
            currentPatrolTarget = NearestPatrolPoint();
            agent.SetDestination(currentPatrolTarget.position);
            return;
        }

        // Stop at attackRadius — don't crowd the player
        agent.stoppingDistance = attackRadius;
        agent.SetDestination(player.position);

        if (distanceToPlayer <= attackRadius)
            generalAnim?.SetAttacking(true);
        else
        {
            generalAnim?.SetAttacking(false);
            generalAnim?.SetWalking(true);
        }

        attackTimer += Time.deltaTime;

        if (distanceToPlayer <= attackRadius && attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            DealDamage();
        }
    }

    private Transform NearestPatrolPoint()
    {
        if (patrolPointA == null) return patrolPointB;
        if (patrolPointB == null) return patrolPointA;

        float distA = Vector3.Distance(transform.position, patrolPointA.position);
        float distB = Vector3.Distance(transform.position, patrolPointB.position);
        return distA < distB ? patrolPointA : patrolPointB;
    }

    private void DealDamage()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.TakeDamage(damageAmount);
    }

    public void OnPlayerEnteredRoom()
    {
        if (currentState == GeneralState.Dead) return;
        currentState = GeneralState.Chase;
        agent.SetDestination(player.position);
    }

    public void Die()
    {
        if (currentState == GeneralState.Dead) return;
        currentState = GeneralState.Dead;

        StopAllCoroutines();
        agent.enabled = false;
        rb.isKinematic = false;

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // Play death animation
        generalAnim?.PlayDeath();

        // Wait for death animation to complete
        float timeout = 6f;
        float elapsed = 0f;
        while (elapsed < timeout)
        {
            if (generalAnim != null && generalAnim.IsDeathComplete()) break;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Brief delay then stop player movement (camera still works)
        yield return new WaitForSeconds(playerStopDelay);

        PlayerController pc = player?.GetComponent<PlayerController>();
        if (pc != null) pc.enabled = false;

        // Wait for decision delay then show panel
        yield return new WaitForSeconds(decisionTriggerDelay);

        // Stop camera too when panel shows
        CameraController cam = Camera.main?.GetComponent<CameraController>();
        if (cam != null) cam.enabled = false;

        GeneralDecisionTrigger decisionTrigger = GetComponentInChildren<GeneralDecisionTrigger>();
        if (decisionTrigger != null)
            decisionTrigger.ShowPanel();
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
