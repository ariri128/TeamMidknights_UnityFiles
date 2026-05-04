using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Renderer visualRenderer;

    public float distanceMultiplier = 2.2f;
    public float heightMultiplier = 0.9f;
    public float shoulderOffset = 0.4f;
    public float playerScreenOffset = 0.35f;

    public float mouseSensitivity = 0.08f;
    public float minPitch = -15f;
    public float maxPitch = 25f;

    public float moveSmoothness = 8f;
    public float rotateSmoothness = 10f;

    public void SetGamepadLookInput(Vector2 input) { gamepadLook = input; }

    [Header("Camera Collision")]
    [Tooltip("Layers the camera will collide with. Set to Default, Wall, Floor etc.")]
    public LayerMask collisionLayers = ~0;

    [Tooltip("How far the camera pulls in front of the wall it hits.")]
    public float collisionOffset = 0.2f;

    private float yaw;
    private float pitch;
    private Vector3 currentVelocity;
    private Vector2 gamepadLook;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (target == null || visualRenderer == null)
        {
            return;
        }

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        SnapCameraToTarget();

        if (ObjectiveUpdateUI.Instance != null)
        {
            ObjectiveUpdateUI.Instance.ShowMessage("Find your way to the top floor!");
        }
    }

    private void LateUpdate()
    {
        if (target == null || visualRenderer == null)
        {
            return;
        }

        HandleMouseLook();
        UpdateCameraPositionAndRotation(false);
    }

    private void SnapCameraToTarget()
    {
        UpdateCameraPositionAndRotation(true);
    }

    private void UpdateCameraPositionAndRotation(bool instant)
    {
        Bounds bounds = visualRenderer.bounds;
        float characterHeight = bounds.size.y;

        Quaternion orbitRotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 focusPoint = bounds.center + Vector3.up * (characterHeight * 0.15f);
        focusPoint += orbitRotation * Vector3.right * (characterHeight * playerScreenOffset);

        float sideOffset = characterHeight * shoulderOffset;
        float verticalOffset = characterHeight * heightMultiplier;
        float distance = characterHeight * distanceMultiplier;

        Vector3 orbitOffset = orbitRotation * new Vector3(sideOffset, verticalOffset, -distance);
        Vector3 desiredPosition = focusPoint + orbitOffset;

        // ── Camera collision: pull camera in if something is between it and the player ──
        Vector3 direction = desiredPosition - focusPoint;
        float desiredDistance = direction.magnitude;
        float actualDistance = desiredDistance;

        if (Physics.SphereCast(
            focusPoint,
            collisionOffset,
            direction.normalized,
            out RaycastHit hit,
            desiredDistance,
            collisionLayers,
            QueryTriggerInteraction.Ignore))
        {
            actualDistance = Mathf.Max(hit.distance - collisionOffset, 0.1f);
        }

        Vector3 actualPosition = focusPoint + direction.normalized * actualDistance;
        Quaternion desiredRotation = Quaternion.LookRotation(focusPoint - actualPosition);

        if (instant)
        {
            transform.position = actualPosition;
            transform.rotation = desiredRotation;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                actualPosition,
                ref currentVelocity,
                1f / moveSmoothness
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                desiredRotation,
                rotateSmoothness * Time.deltaTime
            );
        }
    }

    private void HandleMouseLook()
    {
        Vector2 delta = Vector2.zero;

        // Mouse input
        if (Mouse.current != null)
            delta = Mouse.current.delta.ReadValue();

        // Gamepad right stick — use higher multiplier since stick is -1 to 1 not pixels
        delta += gamepadLook * 5f;

        yaw += delta.x * mouseSensitivity;
        pitch -= delta.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }
}