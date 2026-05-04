using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingSphereTrigger : MonoBehaviour
{
    [Header("Player")]
    public GameObject playerObject;

    [Header("Panel")]
    public GameObject endingPromptPanel;
    public Button yesButton;
    public Button noButton;

    [Header("Scene")]
    [Tooltip("The single ending scene name in Build Settings.")]
    public string endingSceneName = "Ending";

    private bool playerInside = false;
    private CameraController cameraController;

    private void Start()
    {
        if (endingPromptPanel != null)
            endingPromptPanel.SetActive(false);

        if (yesButton != null) yesButton.onClick.AddListener(OnYesClicked);
        if (noButton != null) noButton.onClick.AddListener(OnNoClicked);

        if (playerObject != null)
            cameraController = Camera.main?.GetComponent<CameraController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("EndingSphereTrigger: Something entered - " + other.gameObject.name);

        if (other.gameObject != playerObject)
        {
            Debug.Log("EndingSphereTrigger: Not the player. Expected: " +
                (playerObject != null ? playerObject.name : "NULL") +
                " Got: " + other.gameObject.name);
            return;
        }

        if (EndingTracker.Instance == null)
        {
            Debug.LogError("EndingSphereTrigger: EndingTracker.Instance is null!");
            return;
        }

        if (!EndingTracker.Instance.AllChoicesMade())
        {
            Debug.Log("EndingSphereTrigger: Not all choices made yet. " +
                "Choices made: " + EndingTracker.Instance.AllChoicesMade());
            return;
        }

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
        if (endingPromptPanel != null)
            endingPromptPanel.SetActive(true);

        SetPlayerControl(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HidePanel()
    {
        if (endingPromptPanel != null)
            endingPromptPanel.SetActive(false);

        SetPlayerControl(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnYesClicked()
    {
        if (endingPromptPanel != null)
            endingPromptPanel.SetActive(false);

        Time.timeScale = 1f;
        SceneManager.LoadScene(endingSceneName, LoadSceneMode.Single);
    }

    private void OnNoClicked()
    {
        HidePanel();
    }

    private void SetPlayerControl(bool enabled)
    {
        if (playerObject == null) return;

        var controller = playerObject.GetComponent<PlayerController>();
        if (controller != null) controller.enabled = enabled;

        if (cameraController == null)
            cameraController = Camera.main?.GetComponent<CameraController>();

        if (cameraController != null) cameraController.enabled = enabled;
    }

    /*
    [Header("Player")]
    public GameObject playerObject;

    [Header("Panel")]
    public GameObject endingPromptPanel;
    public Button yesButton;
    public Button noButton;

    [Header("Scene")]
    [Tooltip("The single ending scene name in Build Settings.")]
    public string endingSceneName = "Ending";

    [Header("Camera Dive")]
    [Tooltip("How long the camera moves forward into the sphere before scene loads.")]
    public float diveDuration = 1.2f;

    [Tooltip("How fast the camera moves toward the sphere.")]
    public float diveSpeed = 5f;

    private bool playerInside = false;
    private CameraController cameraController;

    private void Start()
    {
        if (endingPromptPanel != null)
            endingPromptPanel.SetActive(false);

        if (yesButton != null) yesButton.onClick.AddListener(OnYesClicked);
        if (noButton != null) noButton.onClick.AddListener(OnNoClicked);

        if (playerObject != null)
            cameraController = Camera.main?.GetComponent<CameraController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("EndingSphereTrigger: Something entered - " + other.gameObject.name);

        if (other.gameObject != playerObject)
        {
            Debug.Log("EndingSphereTrigger: Not the player. Expected: " +
                (playerObject != null ? playerObject.name : "NULL") +
                " Got: " + other.gameObject.name);
            return;
        }

        if (EndingTracker.Instance == null)
        {
            Debug.LogError("EndingSphereTrigger: EndingTracker.Instance is null!");
            return;
        }

        if (!EndingTracker.Instance.AllChoicesMade())
        {
            Debug.Log("EndingSphereTrigger: Not all choices made yet. " +
                "Choices made: " + EndingTracker.Instance.AllChoicesMade());
            return;
        }

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
        if (endingPromptPanel != null)
            endingPromptPanel.SetActive(true);

        SetPlayerControl(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HidePanel()
    {
        if (endingPromptPanel != null)
            endingPromptPanel.SetActive(false);

        SetPlayerControl(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnYesClicked()
    {
        if (endingPromptPanel != null)
            endingPromptPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(DiveIntoSphere());
    }

    private void OnNoClicked()
    {
        HidePanel();
    }

    private IEnumerator DiveIntoSphere()
    {
        // Disable camera controller so we can move it manually
        if (cameraController != null) cameraController.enabled = false;

        Camera cam = Camera.main;
        Vector3 diveTarget = transform.position; // center of sphere
        float elapsed = 0f;
        Vector3 startPos = cam.transform.position;

        while (elapsed < diveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / diveDuration);
            cam.transform.position = Vector3.Lerp(startPos, diveTarget, t);
            cam.transform.rotation = Quaternion.Slerp(
                cam.transform.rotation,
                Quaternion.LookRotation(diveTarget - cam.transform.position + Vector3.forward * 0.01f),
                t
            );
            yield return null;
        }

        SceneManager.LoadScene(endingSceneName, LoadSceneMode.Single);
    }

    private void SetPlayerControl(bool enabled)
    {
        if (playerObject == null) return;

        var controller = playerObject.GetComponent<PlayerController>();
        if (controller != null) controller.enabled = enabled;

        if (cameraController == null)
            cameraController = Camera.main?.GetComponent<CameraController>();

        if (cameraController != null) cameraController.enabled = enabled;
    }
    */

    /*
    [Header("Player")]
    public GameObject playerObject;

    [Header("Panel")]
    public GameObject endingPromptPanel;
    public Button yesButton;
    public Button noButton;

    [Header("Scene")]
    [Tooltip("The single ending scene name in Build Settings.")]
    public string endingSceneName = "Ending";

    [Header("Camera Dive")]
    [Tooltip("How long the camera moves forward into the sphere before scene loads.")]
    public float diveDuration = 1.2f;

    [Tooltip("How fast the camera moves toward the sphere.")]
    public float diveSpeed = 5f;

    private bool playerInside = false;
    private CameraController cameraController;

    private void Start()
    {
        if (endingPromptPanel != null)
            endingPromptPanel.SetActive(false);

        if (yesButton != null) yesButton.onClick.AddListener(OnYesClicked);
        if (noButton != null) noButton.onClick.AddListener(OnNoClicked);

        if (playerObject != null)
            cameraController = Camera.main?.GetComponent<CameraController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != playerObject) return;
        if (!EndingTracker.Instance.AllChoicesMade()) return;

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
        if (endingPromptPanel != null)
            endingPromptPanel.SetActive(true);

        SetPlayerControl(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HidePanel()
    {
        if (endingPromptPanel != null)
            endingPromptPanel.SetActive(false);

        SetPlayerControl(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnYesClicked()
    {
        if (endingPromptPanel != null)
            endingPromptPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(DiveIntoSphere());
    }

    private void OnNoClicked()
    {
        HidePanel();
    }

    private IEnumerator DiveIntoSphere()
    {
        // Disable camera controller so we can move it manually
        if (cameraController != null) cameraController.enabled = false;

        Camera cam = Camera.main;
        Vector3 diveTarget = transform.position; // center of sphere
        float elapsed = 0f;
        Vector3 startPos = cam.transform.position;

        while (elapsed < diveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / diveDuration);
            cam.transform.position = Vector3.Lerp(startPos, diveTarget, t);
            cam.transform.rotation = Quaternion.Slerp(
                cam.transform.rotation,
                Quaternion.LookRotation(diveTarget - cam.transform.position + Vector3.forward * 0.01f),
                t
            );
            yield return null;
        }

        SceneManager.LoadScene(endingSceneName, LoadSceneMode.Single);
    }

    private void SetPlayerControl(bool enabled)
    {
        if (playerObject == null) return;

        var controller = playerObject.GetComponent<PlayerController>();
        if (controller != null) controller.enabled = enabled;

        if (cameraController == null)
            cameraController = Camera.main?.GetComponent<CameraController>();

        if (cameraController != null) cameraController.enabled = enabled;
    }
    */
}
