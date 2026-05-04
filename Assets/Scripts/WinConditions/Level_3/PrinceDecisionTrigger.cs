using UnityEngine;
using UnityEngine.UI;

public class PrinceDecisionTrigger : MonoBehaviour
{
    [Header("Decision Panel")]
    public GameObject decisionPanel;
    public Button killButton;
    public Button spareButton;

    [Header("Outcome Panels")]
    public GameObject princeKilledPanel;
    public GameObject princeSparedPanel;

    [Header("Return Buttons")]
    public Button returnFromKillButton;
    public Button returnFromSpareButton;

    [Header("Level Loader")]
    public LevelLoader levelLoader;

    [Header("UI to Hide")]
    public GameObject[] uiElementsToHide;

    [Header("Player")]
    public GameObject playerObject;

    [Header("Locked State UI")]
    [Tooltip("Optional: a small hint shown when the player enters the trigger but hasn't collected everything yet.")]
    public GameObject lockedHintUI;

    private bool isUnlocked = false;
    private bool triggered = false;
    private bool playerIsInside = false;

    private void Start()
    {
        if (decisionPanel != null) decisionPanel.SetActive(false);
        if (princeKilledPanel != null) princeKilledPanel.SetActive(false);
        if (princeSparedPanel != null) princeSparedPanel.SetActive(false);
        if (lockedHintUI != null) lockedHintUI.SetActive(false);

        if (killButton != null) killButton.onClick.AddListener(OnKillChosen);
        if (spareButton != null) spareButton.onClick.AddListener(OnSpareChosen);
        if (returnFromKillButton != null) returnFromKillButton.onClick.AddListener(ReturnToHub);
        if (returnFromSpareButton != null) returnFromSpareButton.onClick.AddListener(ReturnToHub);
    }

    public void Unlock()
    {
        isUnlocked = true;

        if (lockedHintUI != null)
            lockedHintUI.SetActive(false);

        // If the player walked in early and is already standing here, trigger now
        if (playerIsInside && !triggered)
            ShowDecisionPanel();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != playerObject) return;

        playerIsInside = true;

        if (!isUnlocked)
        {
            // Player is here early — show a hint if assigned
            if (lockedHintUI != null)
                lockedHintUI.SetActive(true);
            return;
        }

        if (!triggered)
            ShowDecisionPanel();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != playerObject) return;

        playerIsInside = false;

        if (lockedHintUI != null)
            lockedHintUI.SetActive(false);
    }

    private void ShowDecisionPanel()
    {
        triggered = true;

        Time.timeScale = 0f;

        foreach (GameObject ui in uiElementsToHide)
            if (ui != null) ui.SetActive(false);

        if (decisionPanel != null)
            decisionPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnKillChosen()
    {
        if (decisionPanel != null) decisionPanel.SetActive(false);
        if (princeKilledPanel != null) princeKilledPanel.SetActive(true);
        EndingTracker.Instance?.SetPrinceChoice(true);
    }

    private void OnSpareChosen()
    {
        if (decisionPanel != null) decisionPanel.SetActive(false);
        if (princeSparedPanel != null) princeSparedPanel.SetActive(true);
        EndingTracker.Instance?.SetPrinceChoice(false);
    }

    private void ReturnToHub()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (levelLoader != null)
            levelLoader.LoadNextLevel();
        else
            Debug.LogError("PrinceDecisionTrigger: No LevelLoader assigned!");
    }
}
