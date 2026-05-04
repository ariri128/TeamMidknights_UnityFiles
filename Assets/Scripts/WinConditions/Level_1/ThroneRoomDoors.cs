using UnityEngine;
using System.Collections;

public class ThroneRoomDoors : MonoBehaviour
{
    public static ThroneRoomDoors Instance { get; private set; }

    [Header("Doors")]
    [Tooltip("Drag all the door GameObjects here. They will be deactivated to 'close' them and activated to 'open'.")]
    public GameObject[] doors;

    [Tooltip("If true, the doors slide/rotate open using animation. If false, they just disappear.")]
    public bool useAnimation = false;

    [Tooltip("If useAnimation is true, assign the Animator on your door(s) here.")]
    public Animator[] doorAnimators;

    [Tooltip("The name of the trigger parameter in your door Animator to play the open animation.")]
    public string openAnimationTrigger = "Open";

    [Header("Doors Open Panel")]
    [Tooltip("Panel that pops up when all objectives are complete and doors open.")]
    public GameObject doorsOpenPanel;

    [Tooltip("How long the panel stays visible before hiding.")]
    public float panelDisplayDuration = 3f;

    // Internal access flags
    private bool guardsCleared = false;
    private bool daggerCollected = false;
    private bool narrativeCollected = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        // Makes sure doors start closed
        SetDoorsBlocking(true);
    }

    public void OnAllGuardsCleared()
    {
        guardsCleared = true;
        CheckOpenDoors();
    }

    public void OnDaggerCollected()
    {
        daggerCollected = true;
        CheckOpenDoors();
    }

    public void OnAllNarrativeCollected()
    {
        narrativeCollected = true;
        CheckOpenDoors();
    }

    private void CheckOpenDoors()
    {
        if (guardsCleared && daggerCollected && narrativeCollected)
        {
            OpenDoors();
        }
    }

    private void OpenDoors()
    {
        Debug.Log("Throne room doors opening!");

        if (doorsOpenPanel != null)
            StartCoroutine(ShowPanelForDuration(doorsOpenPanel, panelDisplayDuration));

        if (ObjectiveUpdateUI.Instance != null)
            ObjectiveUpdateUI.Instance.ShowMessage("The Throne Room is Now Open");

        if (useAnimation && doorAnimators.Length > 0)
        {
            foreach (Animator anim in doorAnimators)
            {
                if (anim != null)
                    anim.SetTrigger(openAnimationTrigger);
            }
        }
        else
        {
            // Deactive doors when conditions met
            SetDoorsBlocking(false);
        }
    }

    private IEnumerator ShowPanelForDuration(GameObject panel, float duration)
    {
        panel.SetActive(true);
        yield return new WaitForSeconds(duration);
        panel.SetActive(false);
    }

    private void SetDoorsBlocking(bool blocking)
    {
        foreach (GameObject door in doors)
        {
            if (door != null)
                door.SetActive(blocking);
        }
    }
}
