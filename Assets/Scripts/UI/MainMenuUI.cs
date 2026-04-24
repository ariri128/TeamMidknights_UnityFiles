using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject instructionsPanel;

    public void ShowInstructions()
    {
        instructionsPanel.SetActive(true);
    }

    public void HideInstructions()
    {
        instructionsPanel.SetActive(false);
    }
}
