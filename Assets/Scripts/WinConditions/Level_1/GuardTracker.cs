using UnityEngine;
using System.Collections;

public class GuardTracker : MonoBehaviour
{
    public static GuardTracker Instance { get; private set; }

    [Header("References")]
    [Tooltip("The dagger prefab to spawn when all guards are dead.")]
    public GameObject daggerPrefab;

    [Tooltip("Drag your Player root GameObject here.")]
    public GameObject playerReference;

    [Tooltip("How high above the guard's position to spawn the dagger so it visibly drops to the floor.")]
    public float spawnHeightOffset = 1.2f;

    [Header("Objectives")]
    [Tooltip("The Kill Guards parent objective entry.")]
    public ObjectivesEntryUI killGuardsObjective;

    [Tooltip("The Find Dagger parent objective entry.")]
    public ObjectivesEntryUI findDaggerObjective;

    [Tooltip("The Collect Dagger child count objective entry.")]
    public ObjectivesEntryUI collectDaggerObjective;

    [Header("All Guards Killed Panel")]
    [Tooltip("A panel that pops up when the last guard is killed. Will auto-hide after the duration.")]
    public GameObject allGuardsKilledPanel;

    [Tooltip("How long the panel stays visible before hiding automatically.")]
    public float panelDisplayDuration = 3f;

    private int guardsAlive = 0;
    private bool daggerSpawned = false;
    private Vector3 lastGuardPosition;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void RegisterGuard()
    {
        guardsAlive++;
        Debug.Log("GuardTracker: Guard registered. Total: " + guardsAlive);
    }

    public void ReportGuardDeath(Vector3 guardPosition)
    {
        lastGuardPosition = guardPosition;
        guardsAlive--;

        Debug.Log("GuardTracker: Guard died. " + guardsAlive + " remaining.");

        if (guardsAlive <= 0 && !daggerSpawned)
        {
            SpawnDagger();
        }
    }

    public void OnDaggerPickedUp()
    {
        // Complete dagger objectives
        if (collectDaggerObjective != null)
            collectDaggerObjective.CompleteObjective();

        if (findDaggerObjective != null)
            findDaggerObjective.CompleteObjective();

        if (ObjectiveUpdateUI.Instance != null)
            ObjectiveUpdateUI.Instance.ShowMessage("Dagger Collected");

        ThroneRoomDoors.Instance?.OnDaggerCollected();
    }

    private void SpawnDagger()
    {
        if (daggerPrefab == null)
        {
            Debug.LogError("GuardTracker: Missing dagger prefab!");
            return;
        }

        daggerSpawned = true;

        // Spawn dagger slightly above the guard so it drops down naturally via gravity
        Vector3 spawnPosition = lastGuardPosition + Vector3.up * spawnHeightOffset;

        GameObject dagger = Instantiate(daggerPrefab, spawnPosition, Quaternion.identity);

        // Pass the player reference to the dagger pickup script
        DaggerPickup pickup = dagger.GetComponent<DaggerPickup>();
        if (pickup != null)
            pickup.playerObject = playerReference;

        Debug.Log("All guards defeated! Dagger dropped at last guard position.");

        // Complete kill guards objective
        if (killGuardsObjective != null)
            killGuardsObjective.CompleteObjective();

        if (ObjectiveUpdateUI.Instance != null)
            ObjectiveUpdateUI.Instance.ShowMessage("All Guards Have Been Killed");

        // Show dedicated announcement panel
        if (allGuardsKilledPanel != null)
            StartCoroutine(ShowPanelForDuration(allGuardsKilledPanel, panelDisplayDuration));

        ThroneRoomDoors.Instance?.OnAllGuardsCleared();
    }

    private IEnumerator ShowPanelForDuration(GameObject panel, float duration)
    {
        panel.SetActive(true);
        yield return new WaitForSeconds(duration);
        panel.SetActive(false);
    }
}
