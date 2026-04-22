using UnityEngine;
using UnityEngine.InputSystem;

public class ManaRefill : MonoBehaviour
{
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
        /*
        if (PauseManager.IsPaused)
        {
            return;
        }
        */

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
}
