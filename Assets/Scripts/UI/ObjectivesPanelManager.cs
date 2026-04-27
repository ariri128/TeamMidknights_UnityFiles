using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class ObjectivesPanelManager : MonoBehaviour
{
    public GameObject objectivesPanel;
    public GameObject minimizedPanel;

    public float showDelay = 1.5f;

    private bool isOpen = false;

    private void Start()
    {
        if (objectivesPanel != null)
        {
            objectivesPanel.SetActive(false);
        }

        if (minimizedPanel != null)
        {
            minimizedPanel.SetActive(false);
        }

        StartCoroutine(ShowObjectivesAfterDelay());
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            TogglePanel();
        }
    }

    private IEnumerator ShowObjectivesAfterDelay()
    {
        yield return new WaitForSeconds(showDelay);

        OpenPanel();
    }

    public void TogglePanel()
    {
        if (isOpen)
        {
            MinimizePanel();
        }
        else
        {
            OpenPanel();
        }
    }

    public void OpenPanel()
    {
        isOpen = true;

        if (objectivesPanel != null)
        {
            objectivesPanel.SetActive(true);
        }

        if (minimizedPanel != null)
        {
            minimizedPanel.SetActive(false);
        }
    }

    public void MinimizePanel()
    {
        isOpen = false;

        if (objectivesPanel != null)
        {
            objectivesPanel.SetActive(false);
        }

        if (minimizedPanel != null)
        {
            minimizedPanel.SetActive(true);
        }
    }
}
