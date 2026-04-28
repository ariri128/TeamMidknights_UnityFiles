using UnityEngine;

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

    // Internal access flags
    private bool guardsCleared = false;
    private bool daggerCollected = false;

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

    private void CheckOpenDoors()
    {
        if (guardsCleared && daggerCollected)
        {
            OpenDoors();
        }
    }

    private void OpenDoors()
    {
        Debug.Log("Throne room doors opening!");

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

    private void SetDoorsBlocking(bool blocking)
    {
        foreach (GameObject door in doors)
        {
            if (door != null)
                door.SetActive(blocking);
        }
    }
}
