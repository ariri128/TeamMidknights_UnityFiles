using UnityEngine;

public class PoisonThroneRoomDoors : MonoBehaviour
{
    [Header("Doors")]
    [Tooltip("Drag the throne room door GameObjects here. They will be deactivated when opened.")]
    public GameObject[] doors;

    [Tooltip("If true, plays an animation instead of just disabling the doors.")]
    public bool useAnimation = false;

    [Tooltip("Animators on the doors if useAnimation is true.")]
    public Animator[] doorAnimators;

    [Tooltip("Trigger parameter name in the door Animator.")]
    public string openAnimationTrigger = "Open";

    private void Start()
    {
        SetDoorsBlocking(true);
    }

    public void OpenDoors()
    {
        Debug.Log("Level 3 throne room doors opening!");

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
