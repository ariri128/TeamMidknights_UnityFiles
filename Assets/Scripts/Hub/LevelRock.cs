using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelRock : MonoBehaviour
{
    [Header("Level Info")]
    [Tooltip("The name of the scene to load (must match Build Settings exactly).")]
    public string sceneName;

    [Tooltip("Display name shown on the panel and floating label (e.g. 'Level 1 - The Palace').")]
    public string levelDisplayName = "Level 1";

    [Header("Player")]
    public GameObject playerObject;

    [Header("Panel References")]
    [Tooltip("The panel that asks 'Start level?'")]
    public GameObject levelEntryPanel;

    [Tooltip("Text inside the panel that shows the level name.")]
    public TextMeshProUGUI panelLevelNameText;

    [Tooltip("Yes button — activates the splash portal and closes panel.")]
    public Button yesButton;

    [Tooltip("No button — closes panel and lets player move again.")]
    public Button noButton;

    [Header("Floating Label")]
    [Tooltip("A World Space Canvas child of this rock with a TextMeshPro text on it.")]
    public TextMeshPro floatingLabel;

    [Header("Splash Portal")]
    [Tooltip("The shared SplashPortal in the hub. Each rock tells it which scene to load.")]
    public SplashPortal splashPortal;

    [Tooltip("The 'Jump into the Portal' prompt to show after the player clicks Yes.")]
    public GameObject portalPromptUI;

    private bool playerOnRock = false;

    private void Start()
    {
        // Set floating label text
        if (floatingLabel != null)
            floatingLabel.text = levelDisplayName;

        // Set panel label
        if (panelLevelNameText != null)
            panelLevelNameText.text = levelDisplayName;

        if (levelEntryPanel != null)
            levelEntryPanel.SetActive(false);

        if (yesButton != null) yesButton.onClick.AddListener(OnYesClicked);
        if (noButton != null) noButton.onClick.AddListener(OnNoClicked);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != playerObject) return;

        playerOnRock = true;
        ShowPanel();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != playerObject) return;

        playerOnRock = false;

        // If player walks off without choosing, close the panel
        HidePanel();
    }

    private void ShowPanel()
    {
        if (levelEntryPanel != null)
            levelEntryPanel.SetActive(true);

        DisablePlayerMovement(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HidePanel()
    {
        if (levelEntryPanel != null)
            levelEntryPanel.SetActive(false);

        DisablePlayerMovement(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnYesClicked()
    {
        HidePanel();

        if (splashPortal != null)
            splashPortal.ActivateForLevel(sceneName);

        if (portalPromptUI != null)
            portalPromptUI.SetActive(true);

        Debug.Log($"Splash portal activated for {sceneName}. Jump in to begin!");
    }

    private void OnNoClicked()
    {
        HidePanel();
        if (portalPromptUI != null)
            portalPromptUI.SetActive(false);
    }

    private void DisablePlayerMovement(bool disable)
    {
        if (playerObject == null) return;
        var controller = playerObject.GetComponent<PlayerController>();
        if (controller != null)
            controller.enabled = !disable;
    }
}
