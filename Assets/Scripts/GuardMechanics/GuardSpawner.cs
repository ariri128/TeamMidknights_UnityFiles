using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class GuardSpawner : MonoBehaviour
{
    public GameObject guardPrefab;
    public Transform player;

    public int guardCount = 2;
    public float spawnRadius = 4f;
    public float navMeshSearchDistance = 2f;
    public float verticalSpawnOffset = 0.9f;

    [Header("Sword Drops (Level 2)")]
    [Tooltip("How many guards on this floor should drop a sword when killed. 0 = none.")]
    public int swordDropCount = 0;

    [Tooltip("The sword drop prefab to assign to randomly selected guards.")]
    public GameObject swordDropPrefab;

    [Tooltip("Player reference for sword pickup — passed at runtime.")]
    public GameObject playerObject;

    [Tooltip("Pickup prompt UI for sword (e.g. '[F] Pick up Sword') — passed at runtime.")]
    public GameObject swordPickupPromptUI;

    private void Start()
    {
        SpawnGuards();
    }

    private void SpawnGuards()
    {
        if (guardPrefab == null || player == null)
            return;

        List<GameObject> spawnedGuards = new List<GameObject>();

        for (int i = 0; i < guardCount; i++)
        {
            Vector3 randomPoint = transform.position + new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                0f,
                Random.Range(-spawnRadius, spawnRadius)
            );

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, navMeshSearchDistance, NavMesh.AllAreas))
            {
                Vector3 spawnPosition = hit.position + Vector3.up * verticalSpawnOffset;
                GameObject guard = Instantiate(guardPrefab, spawnPosition, Quaternion.identity);

                GuardAI ai = guard.GetComponent<GuardAI>();
                if (ai != null)
                {
                    ai.player = player;
                    ai.floorCenter = transform.position;
                    ai.floorPatrolRadius = spawnRadius;
                    ai.navMeshSearchDistance = navMeshSearchDistance;
                }

                // Registration handled automatically in GuardHealth.Awake()
                spawnedGuards.Add(guard);
            }
        }

        // Randomly assign sword drops to guards on this floor
        if (swordDropCount > 0 && swordDropPrefab != null && spawnedGuards.Count > 0)
        {
            // Shuffle the list
            for (int i = spawnedGuards.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                GameObject temp = spawnedGuards[i];
                spawnedGuards[i] = spawnedGuards[j];
                spawnedGuards[j] = temp;
            }

            int toAssign = Mathf.Min(swordDropCount, spawnedGuards.Count);
            for (int i = 0; i < toAssign; i++)
            {
                GuardSwordDrop drop = spawnedGuards[i].AddComponent<GuardSwordDrop>();
                drop.swordDropPrefab = swordDropPrefab;
                drop.playerObject = playerObject;
                drop.pickupPromptUI = swordPickupPromptUI;
                drop.spawnHeightOffset = verticalSpawnOffset;
            }
        }
    }
}
