using UnityEngine;
using UnityEngine.UI;

public class GeneralDecisionTrigger : MonoBehaviour
{
    [Header("Decision Panel")]
    public GameObject decisionPanel;
    public Button killButton;
    public Button spareButton;

    [Header("Outcome Panels")]
    public GameObject generalKilledPanel;
    public GameObject generalSparedPanel;

    [Header("Return Buttons")]
    public Button returnFromKillButton;
    public Button returnFromSpareButton;

    [Header("Level Loader")]
    public LevelLoader levelLoader;

    [Header("UI to Hide")]
    [Tooltip("All other UI elements to hide when the panel appears.")]
    public GameObject[] uiElementsToHide;

    [Header("Player")]
    public GameObject playerObject;

    private bool triggered = false;

    private void Start()
    {
        // Keep the collider disabled until the General dies
        GetComponent<Collider>().enabled = false;

        if (decisionPanel != null) decisionPanel.SetActive(false);
        if (generalKilledPanel != null) generalKilledPanel.SetActive(false);
        if (generalSparedPanel != null) generalSparedPanel.SetActive(false);

        if (killButton != null) killButton.onClick.AddListener(OnKillChosen);
        if (spareButton != null) spareButton.onClick.AddListener(OnSpareChosen);
        if (returnFromKillButton != null) returnFromKillButton.onClick.AddListener(ReturnToHub);
        if (returnFromSpareButton != null) returnFromSpareButton.onClick.AddListener(ReturnToHub);
    }

    public void EnableTrigger()
    {
        GetComponent<Collider>().enabled = true;
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
        if (generalKilledPanel != null) generalKilledPanel.SetActive(true);
    }

    private void OnSpareChosen()
    {
        if (decisionPanel != null) decisionPanel.SetActive(false);
        if (generalSparedPanel != null) generalSparedPanel.SetActive(true);
    }

    private void ReturnToHub()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (levelLoader != null)
            levelLoader.LoadNextLevel();
        else
            Debug.LogError("GeneralDecisionTrigger: No LevelLoader assigned!");
    }
}
