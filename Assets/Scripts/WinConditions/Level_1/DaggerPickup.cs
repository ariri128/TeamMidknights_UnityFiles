using UnityEngine;
using UnityEngine.InputSystem;

public class DaggerPickup : MonoBehaviour
{
    [Tooltip("How close the player must be to pick up the dagger.")]
    public float pickupRadius = 2.5f;

    [Tooltip("Drag your Player root GameObject here.")]
    public GameObject playerObject;

    [Tooltip("Optional: a UI prompt GameObject to show when player is in range (e.g. '[F] Pick up Dagger').")]
    public GameObject pickupPromptUI;

    private Transform player;
    private bool playerInRange = false;

    private void OnEnable()
    {
        InputBridge.OnInteractPressed += OnGamepadInteract;
    }

    private void OnDisable()
    {
        InputBridge.OnInteractPressed -= OnGamepadInteract;
    }

    private void OnGamepadInteract()
    {
        // Mirror F key behavior for gamepad
        if (playerInRange)
            PickUp();
    }

    private void Start()
    {
        if (playerObject != null)
            player = playerObject.transform;
        else
            Debug.LogError("DaggerPickup: No player assigned in the Inspector!");

        if (pickupPromptUI != null)
            pickupPromptUI.SetActive(false);
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        playerInRange = distance <= pickupRadius;

        if (pickupPromptUI != null)
            pickupPromptUI.SetActive(playerInRange);

        if (playerInRange && Keyboard.current.fKey.wasPressedThisFrame)
        {
            PickUp();
        }
    }

    private void PickUp()
    {
        if (pickupPromptUI != null)
            pickupPromptUI.SetActive(false);

        // GuardTracker handles objective completion and notifying ThroneRoomDoors
        GuardTracker.Instance?.OnDaggerPickedUp();

        Destroy(gameObject);
    }

    // Draw the pickup radius in the editor to visualize it
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
