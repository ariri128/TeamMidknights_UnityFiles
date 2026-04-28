using UnityEngine;
using UnityEngine.InputSystem;

public class SwordPickup : MonoBehaviour
{
    [Tooltip("How close the player must be to pick up the sword.")]
    public float pickupRadius = 2.5f;

    [Tooltip("Optional UI prompt to show when in range (e.g. '[F] Pick up Sword').")]
    public GameObject pickupPromptUI;

    [Tooltip("Drag your Player root GameObject here — set by GeneralAI automatically if left empty.")]
    public GameObject playerObject;

    private Transform player;

    private void Start()
    {
        if (playerObject != null)
            player = playerObject.transform;
        else
            Debug.LogError("SwordPickup: No player assigned!");

        if (pickupPromptUI != null)
            pickupPromptUI.SetActive(false);
    }

    private void Update()
    {
        if (player == null) return;

        bool inRange = Vector3.Distance(transform.position, player.position) <= pickupRadius;

        if (pickupPromptUI != null)
            pickupPromptUI.SetActive(inRange);

        if (inRange && Keyboard.current.fKey.wasPressedThisFrame)
            PickUp();
    }

    private void PickUp()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
