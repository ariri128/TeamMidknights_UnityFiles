using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [Tooltip("Name of your main menu scene in Build Settings.")]
    public string mainMenuSceneName = "MainMenu";

    [Tooltip("Name of your hub scene in Build Settings.")]
    public string hubSceneName = "Hub";

    public void OnResumeClicked()
    {
        PauseManager pm = FindPauseManager();
        if (pm != null)
            pm.Resume();
    }

    public void OnRestartClicked()
    {
        string levelScene = GetLevelSceneName();

        PauseManager pm = FindPauseManager();
        if (pm != null)
            pm.ExitToScene(levelScene);
        else
        {
            // Fallback if PauseManager not found
            Time.timeScale = 1f;
            SceneManager.LoadScene(levelScene, LoadSceneMode.Single);
        }
    }

    public void OnMainMenuClicked()
    {
        PauseManager pm = FindPauseManager();
        if (pm != null)
            pm.ExitToScene(mainMenuSceneName);
        else
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName, LoadSceneMode.Single);
        }
    }

    public void OnReturnToHubClicked()
    {
        PauseManager pm = FindPauseManager();
        if (pm != null)
            pm.ExitToScene(hubSceneName);
        else
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(hubSceneName, LoadSceneMode.Single);
        }
    }

    private PauseManager FindPauseManager()
    {
        PauseManager[] managers = FindObjectsByType<PauseManager>(FindObjectsSortMode.None);
        if (managers.Length > 0) return managers[0];

        Debug.LogError("PauseMenuUI: Could not find PauseManager in loaded scenes.");
        return null;
    }

    private string GetLevelSceneName()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != gameObject.scene.name)
                return scene.name;
        }

        // Fallback — reload whatever scene index 0 is
        return SceneManager.GetSceneAt(0).name;
    }
}
