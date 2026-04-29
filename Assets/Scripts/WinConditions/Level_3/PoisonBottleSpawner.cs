using UnityEngine;

public class PoisonBottleSpawner : MonoBehaviour
{
    [Tooltip("The poison bottle prefab (needs PoisonIngredientPickup + Rigidbody + Collider).")]
    public GameObject bottlePrefab;

    [Tooltip("One spawn point per room the bottle could appear in.")]
    public Transform[] spawnPoints;

    [Tooltip("Height above the spawn point to drop the bottle from.")]
    public float spawnHeightOffset = 1f;

    [Tooltip("Drag your Player root GameObject here.")]
    public GameObject playerObject;

    [Tooltip("Drag the pickup prompt UI GameObject here (e.g. '[F] Pick up Bottle').")]
    public GameObject pickupPromptUI;

    private void Start()
    {
        SpawnBottle();
    }

    private void SpawnBottle()
    {
        if (bottlePrefab == null || spawnPoints.Length == 0)
        {
            Debug.LogError("PoisonBottleSpawner: Missing bottle prefab or spawn points!");
            return;
        }

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPos = spawnPoints[randomIndex].position + Vector3.up * spawnHeightOffset;

        GameObject bottle = Instantiate(bottlePrefab, spawnPos, Quaternion.identity);

        // Pass player reference to the pickup script
        PoisonIngredientsPickup pickup = bottle.GetComponent<PoisonIngredientsPickup>();
        if (pickup != null)
        {
            pickup.playerObject = playerObject;
            pickup.pickupPromptUI = pickupPromptUI;
        }
    }
}
