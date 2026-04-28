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

    [Header("Attack / Bumper Car")]
    [Tooltip("Distance at which the General deals damage and gets knocked back.")]
    public float attackRadius = 1.5f;

    [Tooltip("Damage dealt to the player on each bump.")]
    public int damageAmount = 35;

    [Tooltip("Seconds between each bump attack.")]
    public float attackCooldown = 1.2f;

    [Tooltip("How hard the General is knocked back after bumping the player.")]
    public float knockbackForce = 6f;

    [Tooltip("Seconds the General is knocked back before resuming the chase.")]
    public float knockbackDuration = 0.4f;

    [Header("Death")]
    [Tooltip("The sword prefab to spawn when the General dies (separate prefab with Rigidbody + Collider).")]
    public GameObject swordDropPrefab;

    [Tooltip("The Transform on the General's hand where the sword will spawn from.")]
    public Transform swordHandTransform;

    [Tooltip("Drag the sm_sword child GameObject here so it gets hidden when the drop prefab spawns.")]
    public GameObject heldSword;

    [Tooltip("How far the General tilts backward when falling. 80-90 degrees looks natural.")]
    public float fallAngle = 85f;

    [Tooltip("How fast the General tips over on death.")]
    public float fallSpeed = 3f;

    [Tooltip("Seconds after death before the player can trigger the decision panel. Gives time to pick up the sword.")]
    public float decisionTriggerDelay = 6f;

    private NavMeshAgent agent;
    private Rigidbody rb;

    private enum GeneralState { Patrol, Chase, Knockback, Dead }
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

        normalSpeed = agent.speed;
        normalAngularSpeed = agent.angularSpeed;
        normalAcceleration = agent.acceleration;
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

            case GeneralState.Knockback:
                break;
        }
    }

    private void HandlePatrol()
    {
        if (currentPatrolTarget == null) return;

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
        // If the player has escaped far enough, give up and go back to patrolling
        if (distanceToPlayer > leashRadius)
        {
            currentState = GeneralState.Patrol;
            currentPatrolTarget = NearestPatrolPoint();
            agent.SetDestination(currentPatrolTarget.position);
            return;
        }

        agent.SetDestination(player.position);

        attackTimer += Time.deltaTime;

        if (distanceToPlayer <= attackRadius && attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            BumpPlayer();
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

    private void BumpPlayer()
    {
        // Deal damage
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.TakeDamage(damageAmount);

        // Knock the General back
        StartCoroutine(KnockbackRoutine());
    }

    private IEnumerator KnockbackRoutine()
    {
        currentState = GeneralState.Knockback;
        agent.ResetPath();
        agent.enabled = false;

        // Push backward using Rigidbody for a frame
        rb.isKinematic = false;
        Vector3 knockDir = (transform.position - player.position).normalized;
        knockDir.y = 0f;
        rb.AddForce(knockDir * knockbackForce, ForceMode.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
        agent.enabled = true;
        agent.Warp(transform.position); // Snap agent back to navmesh position

        currentState = GeneralState.Chase;
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

        // Hide the held sword and spawn the drop prefab
        if (heldSword != null) heldSword.SetActive(false);
        if (swordDropPrefab != null && swordHandTransform != null)
        {
            Instantiate(swordDropPrefab, swordHandTransform.position, swordHandTransform.rotation);
        }
        else if (swordDropPrefab != null)
        {
            Instantiate(swordDropPrefab, transform.position + Vector3.up, Quaternion.identity);
        }

        StartCoroutine(FallBackward());
    }

    private IEnumerator FallBackward()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion fallRotation = Quaternion.Euler(fallAngle, transform.eulerAngles.y, transform.eulerAngles.z);

        float elapsed = 0f;
        float duration = 1f / fallSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(startRotation, fallRotation, elapsed / duration);
            yield return null;
        }

        transform.rotation = fallRotation;

        // Notify the decision trigger that the General is down
        GeneralDecisionTrigger decisionTrigger = GetComponentInChildren<GeneralDecisionTrigger>();
        if (decisionTrigger != null)
            StartCoroutine(EnableDecisionTriggerAfterDelay(decisionTrigger));
    }

    private IEnumerator EnableDecisionTriggerAfterDelay(GeneralDecisionTrigger trigger)
    {
        yield return new WaitForSeconds(decisionTriggerDelay);
        trigger.EnableTrigger();
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
