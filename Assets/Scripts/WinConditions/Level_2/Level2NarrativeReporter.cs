using UnityEngine;

[RequireComponent(typeof(InteractiveObject))]
public class Level2NarrativeReporter : MonoBehaviour
{
    private InteractiveObject interactiveObject;
    private bool hasBeenRead = false;
    private bool wasOpenLastFrame = false;

    private void Awake()
    {
        interactiveObject = GetComponent<InteractiveObject>();
    }

    private void Update()
    {
        if (hasBeenRead) return;
        if (interactiveObject == null) return;

        bool isOpenNow = interactiveObject.IsOpen();

        if (isOpenNow && !wasOpenLastFrame)
        {
            hasBeenRead = true;
            Level2Tracker.Instance?.OnNarrativeItemRead();
        }

        wasOpenLastFrame = isOpenNow;
    }
}
