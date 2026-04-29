using UnityEngine;
using System.Collections.Generic;

public class PoisonTracker : MonoBehaviour
{
    public static PoisonTracker Instance { get; private set; }

    [System.Serializable]
    public class Ingredient
    {
        [Tooltip("Must match the ingredientID on PoisonIngredientPickup, or 'water' for the fountain.")]
        public string id;
        [HideInInspector]
        public bool collected = false;
    }

    [Header("Ingredients")]
    [Tooltip("List every ingredient the player must collect. Include an entry with id 'water' for the fountain.")]
    public Ingredient[] ingredients;

    [Header("Prince Trigger")]
    [Tooltip("Assign the PrinceDecisionTrigger so it gets unlocked when all ingredients are collected.")]
    public PrinceDecisionTrigger princeTrigger;

    private Dictionary<string, Ingredient> ingredientMap = new Dictionary<string, Ingredient>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        foreach (var ingredient in ingredients)
            ingredientMap[ingredient.id] = ingredient;
    }

    public void CollectIngredient(string id)
    {
        if (!ingredientMap.TryGetValue(id, out Ingredient ingredient))
        {
            Debug.LogWarning($"PoisonTracker: Unknown ingredient ID '{id}'");
            return;
        }

        if (ingredient.collected) return;

        ingredient.collected = true;
        Debug.Log($"PoisonTracker: Collected '{id}'");

        CheckAllCollected();
    }

    private void CheckAllCollected()
    {
        foreach (var ingredient in ingredients)
        {
            if (!ingredient.collected) return;
        }

        Debug.Log("All poison ingredients collected! Prince trigger unlocked.");

        if (princeTrigger != null)
            princeTrigger.Unlock();
    }

    public bool IsCollected(string id)
    {
        return ingredientMap.TryGetValue(id, out Ingredient i) && i.collected;
    }
}
