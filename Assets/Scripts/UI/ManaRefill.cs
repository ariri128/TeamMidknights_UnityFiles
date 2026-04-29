using UnityEngine;
using UnityEngine.InputSystem;

public class ManaRefill : MonoBehaviour
{
    public float interactionRange = 2f;
    public int manaPerSecond = 60;

    public Transform player;

    [Tooltip("Prompt shown when the player is within range (e.g. '[F] Refill Mana'). Hides when they leave.")]
    public GameObject interactPromptUI;

    private PlayerMana playerMana;
    private bool wasInRange = false;

    private void Start()
    {
        if (player != null)
            playerMana = player.GetComponent<PlayerMana>();

        if (interactPromptUI != null)
            interactPromptUI.SetActive(false);
    }

    private void Update()
    {
        /*
        if (PauseManager.IsPaused)
        {
            return;
        }
        */

        if (player == null || playerMana == null || Keyboard.current == null)
            return;

        bool inRange = IsPlayerInRange();

        // Show/hide prompt as player enters/exits range
        if (inRange != wasInRange)
        {
            wasInRange = inRange;
            if (interactPromptUI != null)
                interactPromptUI.SetActive(inRange);
        }

        if (!inRange)
            return;

        if (Keyboard.current.fKey.isPressed)
            RefillMana();
    }

    private bool IsPlayerInRange()
    {
        return Vector3.Distance(transform.position, player.position) <= interactionRange;
    }

    private void RefillMana()
    {
        if (playerMana.IsFull())
        {
            return;
        }

        int manaToRestore = Mathf.CeilToInt(manaPerSecond * Time.deltaTime);
        playerMana.RestoreMana(manaToRestore);
    }

    /*
    public float interactionRange = 2f;
    public int manaPerSecond = 60;

    public Transform player;
    private PlayerMana playerMana;

    private void Start()
    {
        if (player != null)
        {
            playerMana = player.GetComponent<PlayerMana>();
        }
    }

    private void Update()
    {
        
        if (PauseManager.IsPaused)
        {
            return;
        }
        

        if (player == null || playerMana == null || Keyboard.current == null)
        {
            return;
        }

        if (!IsPlayerInRange())
        {
            return;
        }

        if (Keyboard.current.fKey.isPressed)
        {
            RefillMana();
        }
    }

    private bool IsPlayerInRange()
    {
        return Vector3.Distance(transform.position, player.position) <= interactionRange;
    }

    private void RefillMana()
    {
        if (playerMana.IsFull())
        {
            return;
        }

        int manaToRestore = Mathf.CeilToInt(manaPerSecond * Time.deltaTime);
        playerMana.RestoreMana(manaToRestore);
    }
    */
}
