using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialTrigger : MonoBehaviour
{
    [Header("Player")]
    public GameObject playerObject;

    [Header("Panel")]
    public GameObject tutorialPromptPanel;
    public Button yesButton;
    public Button noButton;

    [Header("Scene")]
    [Tooltip("Must match the tutorial scene name in Build Settings exactly.")]
    public string tutorialSceneName = "Tutorial";

    private bool playerInside = false;

    private void Start()
    {
        if (tutorialPromptPanel != null)
            tutorialPromptPanel.SetActive(false);

        if (yesButton != null) yesButton.onClick.AddListener(OnYesClicked);
        if (noButton != null) noButton.onClick.AddListener(OnNoClicked);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != playerObject) return;
        playerInside = true;
        ShowPanel();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != playerObject) return;
        playerInside = false;
        HidePanel();
    }

    private void ShowPanel()
    {
        if (tutorialPromptPanel != null)
            tutorialPromptPanel.SetActive(true);

        SetPlayerMovement(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HidePanel()
    {
        if (tutorialPromptPanel != null)
            tutorialPromptPanel.SetActive(false);

        SetPlayerMovement(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnYesClicked()
    {
        HidePanel();
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(tutorialSceneName, LoadSceneMode.Single);
    }

    private void OnNoClicked()
    {
        HidePanel();
    }

    private void SetPlayerMovement(bool enabled)
    {
        if (playerObject == null) return;
        var controller = playerObject.GetComponent<PlayerController>();
        if (controller != null) controller.enabled = enabled;
    }
}
