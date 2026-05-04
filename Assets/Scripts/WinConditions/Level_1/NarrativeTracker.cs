using UnityEngine;

public class NarrativeTracker : MonoBehaviour
{
    public static NarrativeTracker Instance { get; private set; }

    [Header("Settings")]
    [Tooltip("Total number of narrative items in this level.")]
    public int totalNarrativeItems = 6;

    [Header("Objectives")]
    [Tooltip("The parent 'Explore the King's story' objective.")]
    public ObjectivesEntryUI exploreObjective;

    [Tooltip("The child 'Narrative Items Found: 0/6' count objective.")]
    public ObjectivesEntryUI narrativeCountObjective;

    private int itemsRead = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void OnNarrativeItemRead()
    {
        if (itemsRead >= totalNarrativeItems) return;

        itemsRead++;

        // Update count objective
        if (narrativeCountObjective != null)
            narrativeCountObjective.AddProgress(1);

        if (itemsRead >= totalNarrativeItems)
            AllNarrativeCollected();
    }

    private void AllNarrativeCollected()
    {
        // Complete both objectives
        if (narrativeCountObjective != null)
            narrativeCountObjective.CompleteObjective();

        if (exploreObjective != null)
            exploreObjective.CompleteObjective();

        if (ObjectiveUpdateUI.Instance != null)
            ObjectiveUpdateUI.Instance.ShowMessage("All Narrative Items Found");

        // Notify throne room doors
        ThroneRoomDoors.Instance?.OnAllNarrativeCollected();
    }

    public int GetItemsRead() => itemsRead;
}
