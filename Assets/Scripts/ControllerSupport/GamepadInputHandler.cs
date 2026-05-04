using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadInputHandler : MonoBehaviour
{
    [Header("Input Asset")]
    public InputActionAsset inputActions;

    [Header("Player Components")]
    public PlayerController playerController;
    public PlayerAttack playerAttack;
    public WaterBallAttack waterBallAttack;
    public TimeSlow timeSlow;
    public TimeRewind timeRewind;
    public PlayerObjectInteraction playerObjectInteraction;
    public PauseManager pauseManager;

    // Cached actions
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction waterAttackAction;
    private InputAction interactAction;
    private InputAction objectivesAction;
    private InputAction rewindAction;
    private InputAction slowAction;
    private InputAction jumpAction;
    private InputAction pauseAction;

    private void Awake()
    {
        if (inputActions == null)
        {
            Debug.LogError("GamepadInputHandler: No Input Action Asset assigned!");
            return;
        }

        var playerMap = inputActions.FindActionMap("Player");

        moveAction = playerMap.FindAction("Move");
        lookAction = playerMap.FindAction("Look");
        waterAttackAction = playerMap.FindAction("WaterAttack");
        interactAction = playerMap.FindAction("Interact");
        objectivesAction = playerMap.FindAction("Objectives");
        rewindAction = playerMap.FindAction("RewindTime");
        slowAction = playerMap.FindAction("SlowTime");
        jumpAction = playerMap.FindAction("Jump");
        pauseAction = playerMap.FindAction("Pause");
    }

    private void OnEnable()
    {
        inputActions?.Enable();
    }

    private void OnDisable()
    {
        inputActions?.Disable();
    }

    private CameraController cameraController;

    private void Start()
    {
        cameraController = Camera.main?.GetComponent<CameraController>();
    }

    private void Update()
    {
        if (Gamepad.current == null) return;

        // Left stick → player movement
        if (moveAction != null && playerController != null && playerController.enabled)
            playerController.SetGamepadMoveInput(moveAction.ReadValue<Vector2>());

        // Right stick → camera look
        if (lookAction != null && cameraController != null && cameraController.enabled)
            cameraController.SetGamepadLookInput(lookAction.ReadValue<Vector2>());

        // Poll button presses every frame — more reliable than event callbacks
        // for inputs that need to be checked in sync with game logic

        // Attack / Water ball
        if (waterAttackAction != null && waterAttackAction.WasPressedThisFrame())
        {
            // Water ball fires during time slow, regular attack otherwise
            if (timeSlow != null && timeSlow.IsSlowActive)
            {
                if (waterBallAttack != null) waterBallAttack.TriggerWaterBall();
            }
            else
            {
                if (playerAttack != null) playerAttack.TriggerAttack();
            }
        }

        // Interact (F key equivalent) — narrative objects and pickups
        if (interactAction != null && interactAction.WasPressedThisFrame())
        {
            if (playerObjectInteraction != null) playerObjectInteraction.TriggerInteract();
            InputBridge.FireInteract();
        }

        // Objectives panel toggle
        if (objectivesAction != null && objectivesAction.WasPressedThisFrame())
        {
            ObjectivesPanelManager panel = FindFirstObjectByType<ObjectivesPanelManager>();
            if (panel != null) panel.TogglePanel();
        }

        // Rewind
        if (rewindAction != null && rewindAction.WasPressedThisFrame())
            if (timeRewind != null) timeRewind.TriggerRewind();

        // Slow time
        if (slowAction != null && slowAction.WasPressedThisFrame())
            if (timeSlow != null) timeSlow.TriggerSlow();

        // Jump
        if (jumpAction != null && jumpAction.WasPressedThisFrame())
            if (playerController != null) playerController.TriggerJump();

        // Pause
        if (pauseAction != null && pauseAction.WasPressedThisFrame())
            if (pauseManager != null) pauseManager.Pause();
    }
}
