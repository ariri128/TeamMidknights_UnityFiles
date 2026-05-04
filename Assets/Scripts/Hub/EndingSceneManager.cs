using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingSceneManager : MonoBehaviour
{
    [Header("Ending Images")]
    [Tooltip("8 ending textures. Index 0 = all spared, Index 7 = all killed. See comment above for full key.")]
    public Texture2D[] endingTextures = new Texture2D[8];

    [Header("Display")]
    [Tooltip("The MeshRenderer on the plane that shows the ending image underwater.")]
    public MeshRenderer endingPlane;

    [Tooltip("The material property name for the texture. Usually _BaseMap for URP or _MainTex for Standard.")]
    public string texturePropertyName = "_BaseMap";

    [Tooltip("How long the ending image is shown before the buttons appear.")]
    public float imageDisplayDuration = 5f;

    [Header("Buttons")]
    [Tooltip("Parent object containing both buttons — hidden initially.")]
    public GameObject buttonsPanel;
    public Button mainMenuButton;
    public Button returnToHubButton;

    [Header("Scenes")]
    public string mainMenuSceneName = "MainMenu";
    public string hubSceneName = "Hub";

    private void Start()
    {
        if (buttonsPanel != null)
            buttonsPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        if (returnToHubButton != null) returnToHubButton.onClick.AddListener(OnReturnToHubClicked);

        ShowEnding();
        StartCoroutine(ShowButtonsAfterDelay());
    }

    private void ShowEnding()
    {
        if (EndingTracker.Instance == null)
        {
            Debug.LogError("EndingSceneManager: EndingTracker not found! Make sure it persists from the Hub.");
            return;
        }

        int index = EndingTracker.Instance.GetEndingIndex();
        Debug.Log($"EndingSceneManager: Showing ending index {index}");

        if (endingTextures == null || index >= endingTextures.Length || endingTextures[index] == null)
        {
            Debug.LogError($"EndingSceneManager: No texture assigned at index {index}!");
            return;
        }

        if (endingPlane != null)
        {
            // Create a material instance so we don't modify the shared material
            Material mat = endingPlane.material;
            mat.SetTexture(texturePropertyName, endingTextures[index]);
        }
    }

    private IEnumerator ShowButtonsAfterDelay()
    {
        yield return new WaitForSeconds(imageDisplayDuration);

        if (buttonsPanel != null)
            buttonsPanel.SetActive(true);
    }

    private void OnReturnToHubClicked()
    {
        // Keep choices intact — player can replay with same or different choices
        SceneManager.LoadScene(hubSceneName, LoadSceneMode.Single);
    }

    private void OnMainMenuClicked()
    {
        // Reset all choices — sphere trigger will be locked again
        if (EndingTracker.Instance != null)
            EndingTracker.Instance.ResetAllChoices();

        SceneManager.LoadScene(mainMenuSceneName, LoadSceneMode.Single);
    }
}
