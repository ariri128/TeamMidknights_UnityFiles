using UnityEngine;

public class NarrativeItemReporter : MonoBehaviour
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

        // Detect the moment it first opens (transition from closed to open)
        if (isOpenNow && !wasOpenLastFrame)
        {
            hasBeenRead = true;
            NarrativeTracker.Instance?.OnNarrativeItemRead();
        }

        wasOpenLastFrame = isOpenNow;
    }
}
