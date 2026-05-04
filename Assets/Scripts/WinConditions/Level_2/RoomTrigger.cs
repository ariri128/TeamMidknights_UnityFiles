using UnityEngine;
using System.Collections;

public class RoomTrigger : MonoBehaviour
{
    [Tooltip("Drag your Player root GameObject here.")]
    public GameObject playerObject;

    [Tooltip("Seconds after player enters before General starts chasing.")]
    public float chaseDelay = 1.5f;

    private GeneralAI general;
    private bool triggered = false;
    private bool isGeneralRoom = false;

    public void SetGeneral(GeneralAI generalAI)
    {
        general = generalAI;
        isGeneralRoom = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != playerObject) return;

        // If this is the general's room, check requirements
        if (isGeneralRoom)
        {
            if (Level2Tracker.Instance != null && !Level2Tracker.Instance.CanEnterGeneralRoom())
            {
                Level2Tracker.Instance.OnLockedRoomAttempt();
                // Block entry — push player back out
                StartCoroutine(PushPlayerBack(other.transform));
                return;
            }
        }

        if (triggered) return;
        triggered = true;

        if (general != null)
            StartCoroutine(AlertGeneralAfterDelay());
    }

    private System.Collections.IEnumerator PushPlayerBack(Transform playerTransform)
    {
        // Wait a frame so physics settles, then move player back outside the trigger
        yield return null;

        // Push player away from the center of this trigger
        Vector3 pushDir = (playerTransform.position - transform.position).normalized;
        pushDir.y = 0f;

        CharacterController cc = playerTransform.GetComponent<CharacterController>();
        if (cc != null)
            cc.Move(pushDir * 1.5f);
        else
            playerTransform.position += pushDir * 1.5f;
    }

    private IEnumerator AlertGeneralAfterDelay()
    {
        yield return new WaitForSeconds(chaseDelay);
        if (general != null)
            general.OnPlayerEnteredRoom();
    }
}
