using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashPortal : MonoBehaviour
{
    [Header("Player")]
    public GameObject playerObject;

    [Header("Visual Feedback (optional)")]
    [Tooltip("A particle effect or glow to enable when the portal is active.")]
    public GameObject activeVisual;

    [Tooltip("A UI prompt shown when the player is near the active portal (e.g. 'Jump in to enter level!').")]
    public GameObject portalPromptUI;

    private string targetScene = "";
    private bool isActive = false;
    private bool playerNearby = false;

    private void Start()
    {
        SetActiveState(false);

        if (portalPromptUI != null)
            portalPromptUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != playerObject) return;

        if (!isActive)
        {
            Debug.Log("SplashPortal: Not yet activated. Stand on a level rock first.");
            return;
        }

        // Player jumped in — load the scene
        LoadLevel();
    }

    /// <summary>
    /// Called by LevelRock when the player clicks Yes.
    /// Unlocks the portal and sets the target scene.
    /// </summary>
    public void ActivateForLevel(string sceneName)
    {
        targetScene = sceneName;
        isActive = true;
        SetActiveState(true);

        Debug.Log($"SplashPortal: Ready to load '{sceneName}'.");
    }

    /// <summary>
    /// Locks the portal again (e.g. if the player clicks No on a different rock).
    /// </summary>
    public void Deactivate()
    {
        targetScene = "";
        isActive = false;
        SetActiveState(false);

        if (portalPromptUI != null)
            portalPromptUI.SetActive(false);
    }

    private void LoadLevel()
    {
        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogError("SplashPortal: No target scene set!");
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
    }

    private void SetActiveState(bool active)
    {
        if (activeVisual != null)
            activeVisual.SetActive(active);
    }
}
