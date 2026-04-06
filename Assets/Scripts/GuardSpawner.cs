using UnityEngine;
using UnityEngine.AI;

public class GuardSpawner : MonoBehaviour
{
    public GameObject guardPrefab;
    public Transform player;

    public int guardCount = 2;
    public float spawnRadius = 4f;
    public float navMeshSearchDistance = 2f;
    public float verticalSpawnOffset = 0.9f;

    private void Start()
    {
        SpawnGuards();
    }

    private void SpawnGuards()
    {
        if (guardPrefab == null || player == null)
        {
            return;
        }

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
            }
        }
    }
}
