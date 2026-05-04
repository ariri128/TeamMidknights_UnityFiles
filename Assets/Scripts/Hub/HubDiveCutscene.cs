using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HubDiveCutscene : MonoBehaviour
{
    public static HubDiveCutscene Instance { get; private set; }

    [Header("References")]
    public CameraController cameraController;
    public PlayerAnimationController playerAnimController;
    public PlayerController playerController;

    [Header("Cutscene Timing")]
    [Tooltip("Time for camera to swing from behind to front of player.")]
    public float cameraSwingDuration = 1.2f;

    [Tooltip("Seconds after camera swing before dive animation plays.")]
    public float diveDelay = 0.3f;

    [Tooltip("Dive animation duration (28 frames at 60fps ≈ 0.47s).")]
    public float diveDuration = 0.47f;

    [Tooltip("How far below the portal the camera dives before scene loads.")]
    public float diveCameraDepth = 3f;

    [Tooltip("How fast the camera moves down during the dive.")]
    public float diveCameraSpeed = 4f;

    private string targetScene;
    private bool isCutscenePlaying = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void TriggerDive(string sceneName)
    {
        if (isCutscenePlaying) return;
        isCutscenePlaying = true;
        targetScene = sceneName;
        StartCoroutine(DiveCutscene());
    }

    private IEnumerator DiveCutscene()
    {
        if (playerController != null) playerController.enabled = false;
        if (cameraController != null) cameraController.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 startCamPos = Camera.main.transform.position;
        Quaternion startCamRot = Camera.main.transform.rotation;

        Vector3 frontPos = transform.position
            + transform.forward * 2.5f
            + Vector3.up * 1.2f;
        Quaternion frontRot = Quaternion.LookRotation(
            (transform.position + Vector3.up * 0.8f) - frontPos
        );

        float elapsed = 0f;
        while (elapsed < cameraSwingDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / cameraSwingDuration);
            Camera.main.transform.position = Vector3.Lerp(startCamPos, frontPos, t);
            Camera.main.transform.rotation = Quaternion.Slerp(startCamRot, frontRot, t);
            yield return null;
        }

        yield return new WaitForSeconds(diveDelay);

        SplashPortal portal = FindObjectsByType<SplashPortal>(FindObjectsSortMode.None)[0];
        if (portal != null)
        {
            Vector3 dir = portal.transform.position - transform.position;
            dir.y = 0f;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);
        }

        if (playerAnimController != null)
            playerAnimController.PlayDive();

        float diveElapsed = 0f;
        Vector3 camStartDive = Camera.main.transform.position;

        while (diveElapsed < diveDuration)
        {
            diveElapsed += Time.deltaTime;
            float t = diveElapsed / diveDuration;

            Camera.main.transform.position = camStartDive
                + transform.forward * (t * 1.5f)
                + Vector3.down * (t * diveCameraDepth);

            Camera.main.transform.rotation = Quaternion.Slerp(
                Camera.main.transform.rotation,
                Quaternion.Euler(45f, Camera.main.transform.eulerAngles.y, 0f),
                t
            );

            yield return null;
        }

        yield return new WaitForSeconds(0.15f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
    }
}
