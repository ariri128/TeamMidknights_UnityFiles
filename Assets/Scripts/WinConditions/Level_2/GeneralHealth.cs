using UnityEngine;
using System.Collections;

public class GeneralHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 300;

    [Header("Damage Flash")]
    public Color flashColor = new Color(1f, 0f, 0f, 0.5f);
    public float flashDuration = 0.15f;

    private int currentHP;
    private GeneralAI generalAI;

    private Renderer[] renderers;
    private Color[] originalColors;
    private Coroutine flashRoutine;

    private void Awake()
    {
        currentHP = maxHP;
        generalAI = GetComponent<GeneralAI>();

        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].material.color;
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0);

        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(DamageFlash());

        if (currentHP <= 0)
            Die();
    }

    private void Die()
    {
        if (generalAI != null)
            generalAI.Die();
    }

    public void KillImmediately()
    {
        currentHP = 0;
        Die();
    }

    private IEnumerator DamageFlash()
    {
        float timer = 0f;
        while (timer < flashDuration)
        {
            timer += Time.deltaTime;
            float t = timer / flashDuration;
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].material.color = Color.Lerp(flashColor, originalColors[i], t);
            yield return null;
        }
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = originalColors[i];
    }
}
