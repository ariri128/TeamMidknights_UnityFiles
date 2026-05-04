using UnityEngine;
using System.Collections;

public class LevelEntryCutscene : MonoBehaviour
{
    [Header("References")]
    public CameraController cameraController;
    public ObjectivesPanelManager objectivesPanelManager;
    public PlayerController playerController;

    [Header("Cutscene Timing")]
    [Tooltip("How far below the player the camera starts (simulates underwater).")]
    public float startDepthBelow = 4f;

    [Tooltip("How long the camera takes to rise up from underwater to front of player.")]
    public float riseDuration = 1.0f;

    [Tooltip("How long the camera takes to swing from front to its normal position behind the player.")]
    public float swingToBackDuration = 1.4f;

    [Tooltip("Seconds to wait after camera settles before showing objectives and unlocking player.")]
    public float settleDelay = 0.5f;

    private void Start()
    {
        if (objectivesPanelManager != null)
        {
            objectivesPanelManager.StopAllCoroutines();
            objectivesPanelManager.enabled = false;
        }

        if (playerController != null)
            playerController.enabled = false;

        if (cameraController != null)
            cameraController.enabled = false;

        StartCoroutine(EmergeFromWater());
    }

    private IEnumerator EmergeFromWater()
    {
        Renderer vis = GetComponentInChildren<Renderer>();
        float charHeight = vis != null ? vis.bounds.size.y : 1.8f;

        Vector3 underwaterStart = transform.position
            + transform.forward * 1.5f
            + Vector3.down * startDepthBelow;

        Camera.main.transform.position = underwaterStart;
        Camera.main.transform.rotation = Quaternion.LookRotation(
            (transform.position + Vector3.up * 0.8f) - underwaterStart
        );

        float shoulderHeight = charHeight * cameraController.heightMultiplier;
        Vector3 frontPos = transform.position
            + transform.forward * 1.5f
            + Vector3.up * shoulderHeight;

        Quaternion frontRot = Quaternion.LookRotation(
            (transform.position + Vector3.up * shoulderHeight * 0.5f) - frontPos
        );

        float elapsed = 0f;
        while (elapsed < riseDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / riseDuration);
            Camera.main.transform.position = Vector3.Lerp(underwaterStart, frontPos, t);
            Camera.main.transform.rotation = Quaternion.Slerp(
                Camera.main.transform.rotation, frontRot, t
            );
            yield return null;
        }

        Vector3 behindOffset = -transform.forward * (charHeight * cameraController.distanceMultiplier)
            + Vector3.up * (charHeight * cameraController.heightMultiplier)
            + transform.right * (charHeight * cameraController.shoulderOffset);
        Vector3 behindPos = transform.position + behindOffset;
        Quaternion behindRot = Quaternion.LookRotation(
            (transform.position + Vector3.up * (charHeight * 0.5f)) - behindPos
        );

        elapsed = 0f;
        while (elapsed < swingToBackDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / swingToBackDuration);
            Camera.main.transform.position = Vector3.Lerp(frontPos, behindPos, t);
            Camera.main.transform.rotation = Quaternion.Slerp(frontRot, behindRot, t);
            yield return null;
        }

        if (cameraController != null)
            cameraController.enabled = true;

        if (playerController != null)
            playerController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yield return new WaitForSeconds(settleDelay);

        if (objectivesPanelManager != null)
        {
            objectivesPanelManager.MarkAsOpened();
            objectivesPanelManager.enabled = true;
            objectivesPanelManager.OpenPanel();
        }
    }
}
