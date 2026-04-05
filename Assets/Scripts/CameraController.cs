using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Renderer visualRenderer;

    public float distanceMultiplier = 2.2f;
    public float heightMultiplier = 0.9f;
    public float sideMultiplier = 0.18f;

    public float mouseSensitivity = 0.08f;
    public float minPitch = -15f;
    public float maxPitch = 25f;

    public float moveSmoothness = 8f;
    public float rotateSmoothness = 10f;

    private float yaw;
    private float pitch;
    private Vector3 currentVelocity;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    private void LateUpdate()
    {
        if (target == null || visualRenderer == null)
        {
            return;
        }

        HandleMouseLook();

        Bounds bounds = visualRenderer.bounds;
        float characterHeight = bounds.size.y;

        Vector3 focusPoint = bounds.center + Vector3.up * (characterHeight * 0.15f);

        Quaternion orbitRotation = Quaternion.Euler(pitch, yaw, 0f);

        float sideOffset = characterHeight * sideMultiplier;
        float verticalOffset = characterHeight * heightMultiplier;
        float distance = characterHeight * distanceMultiplier;

        Vector3 orbitOffset = orbitRotation * new Vector3(sideOffset, verticalOffset, -distance);
        Vector3 desiredPosition = focusPoint + orbitOffset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref currentVelocity,
            1f / moveSmoothness
        );

        Quaternion desiredRotation = Quaternion.LookRotation(focusPoint - transform.position);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            rotateSmoothness * Time.deltaTime
        );
    }

    private void HandleMouseLook()
    {
        if (Mouse.current == null)
        {
            return;
        }

        Vector2 delta = Mouse.current.delta.ReadValue();

        yaw += delta.x * mouseSensitivity;
        pitch -= delta.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }
}
