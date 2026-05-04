using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    public GameObject popupPanel;
    public float interactionRange = 2f;

    [Tooltip("Prompt shown when the player is within range (e.g. '[F] Interact'). Hides when they leave.")]
    public GameObject interactPromptUI;

    private bool isOpen = false;
    private bool wasInRange = false;

    private void Start()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);

        if (interactPromptUI != null)
            interactPromptUI.SetActive(false);
    }

    private void Update()
    {
        // Prompt is driven by PlayerObjectInteraction
    }

    public void ShowPrompt()
    {
        if (interactPromptUI != null)
            interactPromptUI.SetActive(true);
    }

    public void HidePrompt()
    {
        if (interactPromptUI != null)
            interactPromptUI.SetActive(false);
    }

    public bool IsPlayerInRange(Transform player)
    {
        if (player == null)
        {
            return false;
        }

        return Vector3.Distance(transform.position, player.position) <= interactionRange;
    }

    public void TogglePopup()
    {
        if (popupPanel == null)
        {
            return;
        }

        isOpen = !isOpen;
        popupPanel.SetActive(isOpen);

        // Hide prompt while panel is open, show it again when panel closes
        if (interactPromptUI != null)
            interactPromptUI.SetActive(!isOpen);
    }

    public void ClosePopup()
    {
        if (popupPanel == null)
        {
            return;
        }

        isOpen = false;
        popupPanel.SetActive(false);
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}
