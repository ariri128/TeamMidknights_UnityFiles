using UnityEngine;
using UnityEngine.AI;

public class GuardSpawner : MonoBehaviour
{
    public GameObject guardPrefab;
    public Transform player;

    public int guardCount = 2;
    public float spawnRadius = 8f;
    public float navMeshSearchDistance = 4f;

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
                GameObject guard = Instantiate(guardPrefab, hit.position, Quaternion.identity);

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
