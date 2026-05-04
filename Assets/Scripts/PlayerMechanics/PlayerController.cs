using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public Transform cameraTransform;

    public float moveSpeed = 4.5f;
    public float rotationSpeed = 10f;
    public float jumpHeight = 1.2f;
    public float gravity = -20f;

    public bool canMove = true;

    private CharacterController controller;
    private Vector3 velocity;

    private PlayerAnimationController playerAnimation;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerAnimation = GetComponent<PlayerAnimationController>();
    }

    private void Update()
    {
        if (cameraTransform == null)
        {
            return;
        }

        bool isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (canMove)
        {
            // Keyboard input
            if (Keyboard.current != null)
            {
                if (Keyboard.current.aKey.isPressed) horizontal -= 1f;
                if (Keyboard.current.dKey.isPressed) horizontal += 1f;
                if (Keyboard.current.wKey.isPressed) vertical += 1f;
                if (Keyboard.current.sKey.isPressed) vertical -= 1f;
            }

            // Gamepad left stick — adds on top of keyboard
            horizontal += gamepadMove.x;
            vertical += gamepadMove.y;

            // Clamp so combined input doesn't exceed 1
            horizontal = Mathf.Clamp(horizontal, -1f, 1f);
            vertical = Mathf.Clamp(vertical, -1f, 1f);
        }

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;

        if (playerAnimation != null)
        {
            playerAnimation.SetRunning(canMove && moveDirection.magnitude > 0.1f);
        }

        bool jumpPressed = (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
                         || gamepadJumpPressed;
        gamepadJumpPressed = false; // consume the flag

        if (canMove && jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (playerAnimation != null)
            {
                playerAnimation.PlayJump();
            }
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = moveDirection * moveSpeed;
        finalMove.y = velocity.y;

        controller.Move(finalMove * Time.deltaTime);

        if (playerAnimation != null)
        {
            playerAnimation.SetGrounded(controller.isGrounded);
        }

        if (canMove && moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    // ── Gamepad input passthrough ──
    private Vector2 gamepadMove;
    private Vector2 gamepadLook;
    private bool gamepadJumpPressed;

    public void SetGamepadMoveInput(Vector2 input) { gamepadMove = input; }
    public void SetGamepadLookInput(Vector2 input) { gamepadLook = input; }
    public void TriggerJump() { gamepadJumpPressed = true; }

    public void DisableMovement()
    {
        canMove = false;
        velocity = Vector3.zero;
    }

    public void ResetVelocity()
    {
        velocity = Vector3.zero;
    }
}