using UnityEngine;
using System.Collections.Generic;

public class PoisonTracker : MonoBehaviour
{
    public static PoisonTracker Instance { get; private set; }

    [Header("Spices")]
    public ObjectivesEntryUI spicesCountObjective;
    public ObjectivesEntryUI spicesParentObjective;
    public int totalSpices = 4;

    [Header("Succulents")]
    public ObjectivesEntryUI succulentsCountObjective;
    public ObjectivesEntryUI succulentsParentObjective;
    public int totalSucculents = 5;

    [Header("Water")]
    public ObjectivesEntryUI waterCountObjective;
    public ObjectivesEntryUI waterParentObjective;

    [Header("All Ingredients")]
    [Tooltip("Parent 'Find all the ingredients' objective — completes when spices, succulents and water are all done.")]
    public ObjectivesEntryUI allIngredientsObjective;

    [Header("Poison Bottle")]
    public ObjectivesEntryUI bottleCountObjective;
    public ObjectivesEntryUI bottleParentObjective;

    [Header("Narrative")]
    public ObjectivesEntryUI narrativeCountObjective;
    public ObjectivesEntryUI narrativeParentObjective;
    public int totalNarrativeItems = 6;

    [Header("Throne Room")]
    public PrinceDecisionTrigger princeTrigger;
    public PoisonThroneRoomDoors throneRoomDoors;

    [Tooltip("Panel shown when doors open.")]
    public GameObject doorsOpenPanel;
    public float panelDisplayDuration = 3f;

    private int spicesCollected = 0;
    private int succulentsCollected = 0;
    private bool waterCollected = false;
    private bool bottleCollected = false;
    private int narrativeRead = 0;

    private bool spicesDone = false;
    private bool succulentsDone = false;
    private bool waterDone = false;
    private bool bottleDone = false;
    private bool narrativeDone = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void CollectIngredient(string id)
    {
        switch (id)
        {
            case "spice":
            case "spices":
                OnSpiceCollected();
                break;
            case "succulent":
            case "succulents":
                OnSucculentCollected();
                break;
            case "water":
                OnWaterCollected();
                break;
            case "bottle":
                OnBottleCollected();
                break;
            default:
                Debug.LogWarning($"PoisonTracker: Unknown ingredient ID '{id}'");
                break;
        }
    }

    private void OnSpiceCollected()
    {
        if (spicesDone) return;
        spicesCollected++;

        if (spicesCountObjective != null) spicesCountObjective.AddProgress(1);

        if (spicesCollected >= totalSpices)
        {
            spicesDone = true;
            if (spicesCountObjective != null) spicesCountObjective.CompleteObjective();
            if (spicesParentObjective != null) spicesParentObjective.CompleteObjective();
            CheckAllIngredients();
        }
    }

    private void OnSucculentCollected()
    {
        if (succulentsDone) return;
        succulentsCollected++;

        if (succulentsCountObjective != null) succulentsCountObjective.AddProgress(1);

        if (succulentsCollected >= totalSucculents)
        {
            succulentsDone = true;
            if (succulentsCountObjective != null) succulentsCountObjective.CompleteObjective();
            if (succulentsParentObjective != null) succulentsParentObjective.CompleteObjective();
            CheckAllIngredients();
        }
    }

    private void OnWaterCollected()
    {
        if (waterDone) return;
        waterDone = true;
        waterCollected = true;

        if (waterCountObjective != null) waterCountObjective.CompleteObjective();
        if (waterParentObjective != null) waterParentObjective.CompleteObjective();
        CheckAllIngredients();
    }

    private void OnBottleCollected()
    {
        if (bottleDone) return;
        bottleDone = true;
        bottleCollected = true;

        if (bottleCountObjective != null) bottleCountObjective.CompleteObjective();
        if (bottleParentObjective != null) bottleParentObjective.CompleteObjective();
        CheckAllConditions();
    }

    private void CheckAllIngredients()
    {
        if (spicesDone && succulentsDone && waterDone)
        {
            if (allIngredientsObjective != null) allIngredientsObjective.CompleteObjective();
            CheckAllConditions();
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
            CheckAllConditions();
        }
    }

    private void CheckAllConditions()
    {
        if (!spicesDone || !succulentsDone || !waterDone || !bottleDone || !narrativeDone)
            return;

        Debug.Log("Level 3: All conditions met! Throne room opening.");

        if (throneRoomDoors != null)
            throneRoomDoors.OpenDoors();

        if (princeTrigger != null)
            princeTrigger.Unlock();

        if (doorsOpenPanel != null)
            StartCoroutine(ShowPanel());
    }

    private System.Collections.IEnumerator ShowPanel()
    {
        doorsOpenPanel.SetActive(true);
        yield return new WaitForSeconds(panelDisplayDuration);
        doorsOpenPanel.SetActive(false);
    }

    public bool IsCollected(string id)
    {
        switch (id)
        {
            case "spice": case "spices": return spicesDone;
            case "succulent": case "succulents": return succulentsDone;
            case "water": return waterDone;
            case "bottle": return bottleDone;
            default: return false;
        }
    }
}
