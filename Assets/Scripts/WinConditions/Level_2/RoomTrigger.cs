using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [Tooltip("Drag your Player root GameObject here.")]
    public GameObject playerObject;

    private GeneralAI general;
    private bool triggered = false;

    public void SetGeneral(GeneralAI generalAI)
    {
        general = generalAI;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (other.gameObject != playerObject) return;

        triggered = true;

        if (general != null)
            general.OnPlayerEnteredRoom();
    }
}
