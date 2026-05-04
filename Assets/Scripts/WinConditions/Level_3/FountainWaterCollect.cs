using UnityEngine;
using UnityEngine.InputSystem;

public class FountainWaterCollect : MonoBehaviour
{
    [Header("Collection")]
    [Tooltip("Must match the ingredient ID in PoisonTracker for water (e.g. 'water').")]
    public string ingredientID = "water";

    [Tooltip("How close the player must be to collect the water.")]
    public float collectRadius = 2.5f;

    [Tooltip("Optional UI prompt (e.g. '[F] Collect Water').")]
    public GameObject collectPromptUI;

    [Tooltip("Drag your Player root GameObject here.")]
    public GameObject playerObject;

    [Header("Pulse Effect")]
    public float pulseScale = 1.08f;
    public float pulseSpeed = 2f;

    private Transform player;
    private bool playerInRange = false;
    private bool waterCollected = false;
    private Vector3 originalScale;
    private bool isPulsing = false;

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
        if (playerInRange)
            CollectWater();
    }

    private void Start()
    {
        if (playerObject != null)
            player = playerObject.transform;
        else
            Debug.LogError("FountainWaterCollect: No player assigned!");

        if (collectPromptUI != null)
            collectPromptUI.SetActive(false);

        originalScale = transform.localScale;
    }

    private void Update()
    {
        // Once collected, no need to keep checking
        if (waterCollected) return;
        if (player == null) return;

        bool inRange = Vector3.Distance(transform.position, player.position) <= collectRadius;

        if (inRange != playerInRange)
        {
            playerInRange = inRange;
            isPulsing = inRange;

            if (!inRange)
                transform.localScale = originalScale;

            if (collectPromptUI != null)
                collectPromptUI.SetActive(inRange);
        }

        if (isPulsing)
        {
            float scale = 1f + (pulseScale - 1f) * Mathf.Abs(Mathf.Sin(Time.time * pulseSpeed));
            transform.localScale = originalScale * scale;
        }

        if (playerInRange && Keyboard.current.fKey.wasPressedThisFrame)
            CollectWater();
    }

    private void CollectWater()
    {
        waterCollected = true;

        // Hide prompt and glow — fountain stays in the world for mana refills
        StopPulse();
        if (collectPromptUI != null)
            collectPromptUI.SetActive(false);

        PoisonTracker.Instance?.CollectIngredient(ingredientID);

        Debug.Log("Water collected from fountain.");
    }

    private void StopPulse()
    {
        isPulsing = false;
        transform.localScale = originalScale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, collectRadius);
    }
}
