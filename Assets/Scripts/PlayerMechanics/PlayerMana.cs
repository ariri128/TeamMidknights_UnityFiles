using UnityEngine;

public class PlayerMana : MonoBehaviour
{
    public int maxMana = 300;

    private int currentMana;
    private bool outOfManaMessageShown = false;

    public int CurrentMana => currentMana;
    public int MaxMana => maxMana;

    private void Awake()
    {
        currentMana = maxMana;
    }

    public bool TrySpendMana(int amount)
    {
        if (currentMana < amount)
        {
            ShowOutOfManaMessageOnce();
            return false;
        }

        currentMana -= amount;
        currentMana = Mathf.Max(currentMana, 0);

        Debug.Log("Mana used. Current mana: " + currentMana);

        if (currentMana <= 0)
        {
            ShowOutOfManaMessageOnce();
        }

        return true;
    }

    public void RestoreMana(int amount)
    {
        currentMana += amount;
        currentMana = Mathf.Min(currentMana, maxMana);

        Debug.Log("Mana restored. Current mana: " + currentMana);

        if (currentMana > 0)
        {
            outOfManaMessageShown = false;
        }
    }

    public void SetMana(int value)
    {
        currentMana = Mathf.Clamp(value, 0, maxMana);

        if (currentMana > 0)
        {
            outOfManaMessageShown = false;
        }
    }

    private void ShowOutOfManaMessageOnce()
    {
        if (outOfManaMessageShown)
        {
            return;
        }

        outOfManaMessageShown = true;

        if (ObjectiveUpdateUI.Instance != null)
        {
            ObjectiveUpdateUI.Instance.ShowMessage("Out of mana. Refill it at the fountain.");
        }
    }
}
