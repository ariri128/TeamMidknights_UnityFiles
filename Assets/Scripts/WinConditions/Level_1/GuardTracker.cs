using UnityEngine;

public class GuardTracker : MonoBehaviour
{
    public static GuardTracker Instance { get; private set; }

    [Header("References")]
    [Tooltip("The dagger prefab to spawn when all guards are dead.")]
    public GameObject daggerPrefab;

    [Tooltip("Drag your Player root GameObject here.")]
    public GameObject playerReference;

    [Tooltip("How high above the guard's position to spawn the dagger so it visibly drops to the floor.")]
    public float spawnHeightOffset = 1.2f;

    private int guardsAlive = 0;
    private bool daggerSpawned = false;
    private Vector3 lastGuardPosition;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void RegisterGuard()
    {
        guardsAlive++;
    }

    public void ReportGuardDeath(Vector3 guardPosition)
    {
        lastGuardPosition = guardPosition;
        guardsAlive--;

        if (guardsAlive <= 0 && !daggerSpawned)
        {
            SpawnDagger();
        }
    }

    private void SpawnDagger()
    {
        if (daggerPrefab == null)
        {
            Debug.LogError("GuardTracker: Missing dagger prefab!");
            return;
        }

        daggerSpawned = true;

        // Spawn dagger slightly above the guard so it drops down naturally via gravity
        Vector3 spawnPosition = lastGuardPosition + Vector3.up * spawnHeightOffset;

        GameObject dagger = Instantiate(daggerPrefab, spawnPosition, Quaternion.identity);

        // Pass the player reference to the dagger pickup script
        DaggerPickup pickup = dagger.GetComponent<DaggerPickup>();
        if (pickup != null)
            pickup.playerObject = playerReference;

        Debug.Log("All guards defeated! Dagger dropped at last guard position.");
    }
}
