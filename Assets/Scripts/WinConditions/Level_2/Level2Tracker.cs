using UnityEngine;
using System.Collections;

public class Level2Tracker : MonoBehaviour
{
    public static Level2Tracker Instance { get; private set; }

    [Header("Swords")]
    [Tooltip("How many sword-carrying guards spawn (4).")]
    public int totalSwords = 4;
    public ObjectivesEntryUI swordsCountObjective;
    public ObjectivesEntryUI swordsParentObjective;

    [Header("Narrative")]
    public int totalNarrativeItems = 6;
    public ObjectivesEntryUI narrativeCountObjective;
    public ObjectivesEntryUI narrativeParentObjective;

    [Header("Room Access")]
    [Tooltip("The RoomTrigger for the room the General spawns in. Assigned at runtime by GeneralSpawner.")]
    public RoomTrigger generalRoomTrigger;

    [Tooltip("Panel shown when player tries to enter general's room without meeting requirements.")]
    public GameObject lockedRoomPanel;
    public float lockedPanelDuration = 2.5f;

    // Runtime state
    private int swordsCollected = 0;
    private int narrativeRead = 0;
    private bool swordsDone = false;
    private bool narrativeDone = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void OnSwordCollected()
    {
        if (swordsDone) return;
        swordsCollected++;

        if (swordsCountObjective != null) swordsCountObjective.AddProgress(1);

        if (swordsCollected >= totalSwords)
        {
            swordsDone = true;
            if (swordsCountObjective != null) swordsCountObjective.CompleteObjective();
            if (swordsParentObjective != null) swordsParentObjective.CompleteObjective();
        }
    }

    public void OnNarrativeItemRead()
    {
        if (narrativeDone) return;
        narrativeRead++;

        if (narrativeCountObjective != null) narrativeCountObjective.AddProgress(1);

        if (narrativeRead >= totalNarrativeItems)
        {
            narrativeDone = true;
            if (narrativeCountObjective != null) narrativeCountObjective.CompleteObjective();
            if (narrativeParentObjective != null) narrativeParentObjective.CompleteObjective();
        }
    }

    public bool CanEnterGeneralRoom()
    {
        return swordsDone && narrativeDone;
    }

    public void OnLockedRoomAttempt()
    {
        if (lockedRoomPanel != null)
            StartCoroutine(ShowLockedPanel());
    }

    private IEnumerator ShowLockedPanel()
    {
        lockedRoomPanel.SetActive(true);
        yield return new WaitForSeconds(lockedPanelDuration);
        lockedRoomPanel.SetActive(false);
    }

    public bool SwordsDone => swordsDone;
    public bool NarrativeDone => narrativeDone;
}
