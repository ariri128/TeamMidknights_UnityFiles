using UnityEngine;
using UnityEngine.InputSystem;

public class PoisonIngredientsPickup : MonoBehaviour
{
    [Header("Ingredient Info")]
    [Tooltip("Unique ID matching an entry in PoisonTracker (e.g. 'herbs', 'mushroom', 'bottle').")]
    public string ingredientID;

    [Header("Pickup")]
    [Tooltip("How close the player must be to pick up this item.")]
    public float pickupRadius = 2.5f;

    [Tooltip("Optional UI prompt to show when in range (e.g. '[F] Pick up Herbs').")]
    public GameObject pickupPromptUI;

    [Tooltip("Drag your Player root GameObject here.")]
    public GameObject playerObject;

    [Header("Pulse Effect")]
    [Tooltip("How much the object scales up at the peak of the pulse.")]
    public float pulseScale = 1.15f;

    [Tooltip("How fast the pulse oscillates.")]
    public float pulseSpeed = 2.5f;

    [Header("Glow Effect")]
    [Tooltip("Glow color when the player is in range.")]
    public Color glowColor = new Color(1f, 0.85f, 0.2f, 1f);

    [Tooltip("Emission intensity — higher = brighter glow.")]
    public float glowIntensity = 2f;

    [Tooltip("If true, uses an outline mesh overlay instead of emission. Use this for objects with custom shaders that don't support emission.")]
    public bool useOutlineMesh = false;

    [Tooltip("How much bigger the outline mesh is compared to the original. 1.08 = 8 percent bigger.")]
    public float outlineScale = 1.08f;

    private Transform player;
    private bool playerInRange = false;
    private Vector3 originalScale;
    private bool isPulsing = false;
    private Renderer[] renderers;
    private GameObject outlineMeshInstance;

    private void Start()
    {
        if (playerObject != null)
            player = playerObject.transform;
        else
            Debug.LogError($"PoisonIngredientPickup ({ingredientID}): No player assigned!");

        if (pickupPromptUI != null)
            pickupPromptUI.SetActive(false);

        originalScale = transform.localScale;

        renderers = GetComponentsInChildren<Renderer>();

        // Build outline mesh if needed
        if (useOutlineMesh)
            BuildOutlineMesh();

        SetGlow(false);
    }

    private void BuildOutlineMesh()
    {
        // Duplicate all child mesh renderers into a single outline parent
        outlineMeshInstance = new GameObject("OutlineMesh");
        outlineMeshInstance.transform.SetParent(transform);
        outlineMeshInstance.transform.localPosition = Vector3.zero;
        outlineMeshInstance.transform.localRotation = Quaternion.identity;
        outlineMeshInstance.transform.localScale = Vector3.one * outlineScale;

        // Create a simple emissive material for the outline
        Material outlineMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        outlineMat.SetFloat("_Cull", 1); // Front face culling so it only shows from outside
        outlineMat.EnableKeyword("_EMISSION");
        outlineMat.SetColor("_EmissionColor", Color.black);
        outlineMat.SetColor("_BaseColor", Color.black);

        foreach (MeshFilter mf in GetComponentsInChildren<MeshFilter>())
        {
            GameObject copy = new GameObject("OutlinePart");
            copy.transform.SetParent(outlineMeshInstance.transform);
            copy.transform.localPosition = mf.transform.localPosition;
            copy.transform.localRotation = mf.transform.localRotation;
            copy.transform.localScale = mf.transform.localScale;

            copy.AddComponent<MeshFilter>().sharedMesh = mf.sharedMesh;
            copy.AddComponent<MeshRenderer>().material = outlineMat;
        }

        outlineMeshInstance.SetActive(false);
    }

    private void Update()
    {
        if (player == null) return;

        bool inRange = Vector3.Distance(transform.position, player.position) <= pickupRadius;

        if (inRange != playerInRange)
        {
            playerInRange = inRange;
            isPulsing = inRange;

            if (!inRange)
                transform.localScale = originalScale;

            if (pickupPromptUI != null)
                pickupPromptUI.SetActive(inRange);

            SetGlow(inRange);
        }

        if (isPulsing)
        {
            float scale = 1f + (pulseScale - 1f) * Mathf.Abs(Mathf.Sin(Time.time * pulseSpeed));
            transform.localScale = originalScale * scale;
        }

        if (playerInRange && Keyboard.current.fKey.wasPressedThisFrame)
            PickUp();
    }

    private void PickUp()
    {
        // Hide prompt and glow before destroying
        if (pickupPromptUI != null)
            pickupPromptUI.SetActive(false);

        SetGlow(false);

        PoisonTracker.Instance?.CollectIngredient(ingredientID);


        Destroy(gameObject);
    }

    private void SetGlow(bool on)
    {
        if (useOutlineMesh)
        {
            // Outline mesh mode — just show/hide the scaled overlay
            if (outlineMeshInstance != null)
            {
                outlineMeshInstance.SetActive(on);
                if (on)
                {
                    // Set the emission color on all outline parts
                    foreach (var r in outlineMeshInstance.GetComponentsInChildren<Renderer>())
                    {
                        r.material.EnableKeyword("_EMISSION");
                        r.material.SetColor("_EmissionColor", glowColor * glowIntensity);
                    }
                }
            }
        }
        else
        {
            // Standard emission mode for Shader Graph materials
            if (renderers == null) return;
            Color emission = on ? glowColor * glowIntensity : Color.black;
            foreach (var r in renderers)
            {
                if (on)
                    r.material.EnableKeyword("_EMISSION");
                else
                    r.material.DisableKeyword("_EMISSION");
                r.material.SetColor("_EmissionColor", emission);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
