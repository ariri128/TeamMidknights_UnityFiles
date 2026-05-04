using UnityEngine;

public class GuardSwordDrop : MonoBehaviour
{
    [Tooltip("The sword pickup prefab to spawn on death.")]
    public GameObject swordDropPrefab;

    [Tooltip("Height offset above guard position to spawn sword.")]
    public float spawnHeightOffset = 1.0f;

    [Tooltip("Player reference — passed to SwordPickup on spawn.")]
    public GameObject playerObject;

    [Tooltip("The pickup prompt UI to show when player is in range (e.g. '[F] Pick up Sword').")]
    public GameObject pickupPromptUI;

    public void DropSword()
    {
        if (swordDropPrefab == null) return;

        Vector3 spawnPos = transform.position + Vector3.up * spawnHeightOffset;
        GameObject sword = Instantiate(swordDropPrefab, spawnPos, Quaternion.identity);

        SwordPickup pickup = sword.GetComponent<SwordPickup>();
        if (pickup != null)
        {
            pickup.playerObject = playerObject;
            pickup.pickupPromptUI = pickupPromptUI;
        }
    }
}
