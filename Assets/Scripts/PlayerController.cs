using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public Transform cameraTransform;

    public float moveSpeed = 220f;
    public float rotationSpeed = 8f;
    public float jumpHeight = 16f;
    public float gravity = -160f;

    private CharacterController controller;
    private Vector3 velocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovement();
        HandleGravityAndJump();
    }

    private void HandleMovement()
    {
        if (Keyboard.current == null || cameraTransform == null)
        {
            return;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (Keyboard.current.aKey.isPressed) horizontal -= 1f;
        if (Keyboard.current.dKey.isPressed) horizontal += 1f;
        if (Keyboard.current.wKey.isPressed) vertical += 1f;
        if (Keyboard.current.sKey.isPressed) vertical -= 1f;

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;

        if (moveDirection.magnitude > 0.1f)
        {
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);

            if (moveDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }

    private void HandleGravityAndJump()
    {
        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
