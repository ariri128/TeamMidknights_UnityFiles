using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class ObjectivesPanelManager : MonoBehaviour
{
    public GameObject objectivesPanel;
    public GameObject minimizedPanel;

    public float showDelay = 1.5f;

    private bool isOpen = false;
    private bool startHasRun = false;

    // Static so it survives the component being disabled and re-enabled
    private static bool alreadyOpened = false;

    private void OnDestroy()
    {
        alreadyOpened = false;
        startHasRun = false;
    }

    private void Awake()
    {
        // Hide panels immediately in Awake so they never flash visible
        if (objectivesPanel != null)
            objectivesPanel.SetActive(false);

        if (minimizedPanel != null)
            minimizedPanel.SetActive(false);
    }

    private void Start()
    {
        startHasRun = true;
    }

    private void OnEnable()
    {

        if (alreadyOpened) return;

        if (!startHasRun) return;

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

    public void MarkAsOpened()
    {
        alreadyOpened = true;
    }

    public void OpenPanel()
    {
        alreadyOpened = true;
        isOpen = true;

        if (objectivesPanel != null)
            objectivesPanel.SetActive(true);

        if (minimizedPanel != null)
            minimizedPanel.SetActive(false);
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
