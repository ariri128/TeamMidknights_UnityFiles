using UnityEngine;
using UnityEngine.UI;

public class GeneralSpawner : MonoBehaviour
{
    [Header("General")]
    [Tooltip("The General prefab (has GeneralAI, GeneralHealth on it).")]
    public GameObject generalPrefab;

    [Tooltip("Drag the Player root GameObject here.")]
    public GameObject playerObject;

    [Header("Room A")]
    public Transform spawnPointA;
    public RoomTrigger roomTriggerA;
    public Transform patrolA1;
    public Transform patrolA2;

    [Header("Room B")]
    public Transform spawnPointB;
    public RoomTrigger roomTriggerB;
    public Transform patrolB1;
    public Transform patrolB2;

    [Header("Decision UI (scene Canvas references)")]
    public GameObject decisionPanel;
    public Button killButton;
    public Button spareButton;
    public GameObject generalKilledPanel;
    public GameObject generalSparedPanel;
    public Button returnFromKillButton;
    public Button returnFromSpareButton;
    public LevelLoader levelLoader;
    public GameObject[] uiElementsToHide;

    private void Start()
    {
        SpawnGeneral();
    }

    private void SpawnGeneral()
    {
        bool spawnInA = Random.value < 0.5f;

        Transform chosenSpawn = spawnInA ? spawnPointA : spawnPointB;
        RoomTrigger chosenRoom = spawnInA ? roomTriggerA : roomTriggerB;
        Transform patrol1 = spawnInA ? patrolA1 : patrolB1;
        Transform patrol2 = spawnInA ? patrolA2 : patrolB2;

        GameObject general = Instantiate(generalPrefab, chosenSpawn.position, chosenSpawn.rotation);

        // Pass references to GeneralAI
        GeneralAI ai = general.GetComponent<GeneralAI>();
        if (ai != null)
        {
            ai.player = playerObject.transform;
            ai.roomTrigger = chosenRoom;
            ai.patrolPointA = patrol1;
            ai.patrolPointB = patrol2;

            if (chosenRoom != null)
                chosenRoom.SetGeneral(ai);
        }

        // Pass all scene UI references to the GeneralDecisionTrigger on the prefab
        GeneralDecisionTrigger decisionTrigger = general.GetComponentInChildren<GeneralDecisionTrigger>();
        if (decisionTrigger != null)
        {
            decisionTrigger.playerObject = playerObject;
            decisionTrigger.decisionPanel = decisionPanel;
            decisionTrigger.killButton = killButton;
            decisionTrigger.spareButton = spareButton;
            decisionTrigger.generalKilledPanel = generalKilledPanel;
            decisionTrigger.generalSparedPanel = generalSparedPanel;
            decisionTrigger.returnFromKillButton = returnFromKillButton;
            decisionTrigger.returnFromSpareButton = returnFromSpareButton;
            decisionTrigger.levelLoader = levelLoader;
            decisionTrigger.uiElementsToHide = uiElementsToHide;
        }
    }
}
