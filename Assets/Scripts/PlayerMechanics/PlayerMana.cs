using UnityEngine;
using System.Collections;

public class PlayerMana : MonoBehaviour
{
    public int maxMana = 300;

    [Tooltip("If true, mana is never consumed (used in tutorial).")]
    public bool infiniteMana = false;

    [Header("Out of Mana Panel")]
    [Tooltip("Panel to show briefly when the player runs out of mana.")]
    public GameObject outOfManaPanel;

    [Tooltip("How long the panel stays visible.")]
    public float outOfManaPanelDuration = 2.5f;

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
        if (infiniteMana) return true;

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
            return;

        outOfManaMessageShown = true;

        if (ObjectiveUpdateUI.Instance != null)
            ObjectiveUpdateUI.Instance.ShowMessage("Out of mana. Refill it at the fountain.");

        if (outOfManaPanel != null)
            StartCoroutine(ShowOutOfManaPanelRoutine());
    }

    private IEnumerator ShowOutOfManaPanelRoutine()
    {
        outOfManaPanel.SetActive(true);
        yield return new WaitForSeconds(outOfManaPanelDuration);
        outOfManaPanel.SetActive(false);
    }

    public bool IsFull()
    {
        return currentMana >= maxMana;
    }
}
