using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KingDecisionUI : MonoBehaviour
{
    [Header("Decision Panel")]
    [Tooltip("The panel asking the player what to do with the king.")]
    public GameObject decisionPanel;

    [Tooltip("The 'Kill the King' button.")]
    public Button killButton;

    [Tooltip("The 'Spare the King' button.")]
    public Button spareButton;

    [Header("Outcome Panels")]
    [Tooltip("Panel shown after the player chooses to kill the king.")]
    public GameObject kingKilledPanel;

    [Tooltip("Panel shown after the player chooses to spare the king.")]
    public GameObject kingSparedPanel;

    [Header("Return Buttons")]
    [Tooltip("'Return to Hub' button inside the King Killed panel.")]
    public Button returnFromKillButton;

    [Tooltip("'Return to Hub' button inside the King Spared panel.")]
    public Button returnFromSpareButton;

    [Header("Level Loader")]
    [Tooltip("Assign the GameObject that has the LevelLoader script on it.")]
    public LevelLoader levelLoader;

    [Header("Player & UI")]
    [Tooltip("Drag your Player root GameObject here.")]
    public GameObject playerObject;

    [Tooltip("Drag every other UI GameObject in the Canvas here that should hide when the panel appears (e.g. health bar, objectives panel, crosshair).")]
    public GameObject[] uiElementsToHide;

    private bool triggered = false;

    private void Start()
    {
        if (decisionPanel != null) decisionPanel.SetActive(false);
        if (kingKilledPanel != null) kingKilledPanel.SetActive(false);
        if (kingSparedPanel != null) kingSparedPanel.SetActive(false);

        if (killButton != null) killButton.onClick.AddListener(OnKillChosen);
        if (spareButton != null) spareButton.onClick.AddListener(OnSpareChosen);
        if (returnFromKillButton != null) returnFromKillButton.onClick.AddListener(ReturnToHub);
        if (returnFromSpareButton != null) returnFromSpareButton.onClick.AddListener(ReturnToHub);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (other.gameObject != playerObject) return;

        triggered = true;
        ShowDecisionPanel();
    }

    private void ShowDecisionPanel()
    {
        // Pause everything — guards, physics, and animations
        Time.timeScale = 0f;

        // Hide all other in-game UI
        SetOtherUIVisible(false);

        // Show decision panel
        if (decisionPanel != null)
            decisionPanel.SetActive(true);

        // Unlock cursor so the player can click buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnKillChosen()
    {
        if (decisionPanel != null) decisionPanel.SetActive(false);
        if (kingKilledPanel != null) kingKilledPanel.SetActive(true);
        EndingTracker.Instance?.SetKingChoice(true);
    }

    private void OnSpareChosen()
    {
        if (decisionPanel != null) decisionPanel.SetActive(false);
        if (kingSparedPanel != null) kingSparedPanel.SetActive(true);
        EndingTracker.Instance?.SetKingChoice(false);
    }

    private void ReturnToHub()
    {
        // Restore time before changing scene
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (levelLoader != null)
            levelLoader.LoadNextLevel();
        else
            Debug.LogError("KingDecisionUI: No LevelLoader assigned!");
    }

    private void SetOtherUIVisible(bool visible)
    {
        foreach (GameObject ui in uiElementsToHide)
        {
            if (ui != null)
                ui.SetActive(visible);
        }
    }

    /*
    [Header("Decision Panel")]
    [Tooltip("The panel asking the player what to do with the king.")]
    public GameObject decisionPanel;

    [Tooltip("The 'Kill the King' button.")]
    public Button killButton;

    [Tooltip("The 'Spare the King' button.")]
    public Button spareButton;

    [Header("Outcome Panels")]
    [Tooltip("Panel shown after the player chooses to kill the king.")]
    public GameObject kingKilledPanel;

    [Tooltip("Panel shown after the player chooses to spare the king.")]
    public GameObject kingSparedPanel;

    [Header("Return Buttons")]
    [Tooltip("'Return to Hub' button inside the King Killed panel.")]
    public Button returnFromKillButton;

    [Tooltip("'Return to Hub' button inside the King Spared panel.")]
    public Button returnFromSpareButton;

    [Header("Level Loader")]
    [Tooltip("Assign the GameObject that has the LevelLoader script on it.")]
    public LevelLoader levelLoader;

    [Header("Player & UI")]
    [Tooltip("Drag your Player root GameObject here.")]
    public GameObject playerObject;

    [Tooltip("Drag every other UI GameObject in the Canvas here that should hide when the panel appears (e.g. health bar, objectives panel, crosshair).")]
    public GameObject[] uiElementsToHide;

    private bool triggered = false;

    private void Start()
    {
        if (decisionPanel != null) decisionPanel.SetActive(false);
        if (kingKilledPanel != null) kingKilledPanel.SetActive(false);
        if (kingSparedPanel != null) kingSparedPanel.SetActive(false);

        if (killButton != null) killButton.onClick.AddListener(OnKillChosen);
        if (spareButton != null) spareButton.onClick.AddListener(OnSpareChosen);
        if (returnFromKillButton != null) returnFromKillButton.onClick.AddListener(ReturnToHub);
        if (returnFromSpareButton != null) returnFromSpareButton.onClick.AddListener(ReturnToHub);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (other.gameObject != playerObject) return;

        triggered = true;
        ShowDecisionPanel();
    }

    private void ShowDecisionPanel()
    {
        // Pause everything — guards, physics, and animations
        Time.timeScale = 0f;

        // Hide all other in-game UI
        SetOtherUIVisible(false);

        // Show decision panel
        if (decisionPanel != null)
            decisionPanel.SetActive(true);

        // Unlock cursor so the player can click buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnKillChosen()
    {
        if (decisionPanel != null) decisionPanel.SetActive(false);
        if (kingKilledPanel != null) kingKilledPanel.SetActive(true);
    }

    private void OnSpareChosen()
    {
        if (decisionPanel != null) decisionPanel.SetActive(false);
        if (kingSparedPanel != null) kingSparedPanel.SetActive(true);
    }

    private void ReturnToHub()
    {
        // Restore time before changing scene
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (levelLoader != null)
            levelLoader.LoadNextLevel();
        else
            Debug.LogError("KingDecisionUI: No LevelLoader assigned!");
    }

    private void SetOtherUIVisible(bool visible)
    {
        foreach (GameObject ui in uiElementsToHide)
        {
            if (ui != null)
                ui.SetActive(visible);
        }
    }
    */
}
