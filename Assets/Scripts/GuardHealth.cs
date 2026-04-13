using UnityEngine;
using System.Collections;

public class GuardHealth : MonoBehaviour
{
    public int maxHP = 100;

    // Damage flash
    public Color flashColor = new Color(1f, 0f, 0f, 0.5f);
    public float flashDuration = 0.15f;

    private int currentHP;

    private Renderer[] renderers;
    private Color[] originalColors;

    private Coroutine flashRoutine;

    private void Awake()
    {
        currentHP = maxHP;

        renderers = GetComponentsInChildren<Renderer>();

        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0);

        Debug.Log(gameObject.name + " took damage. Guard HP: " + currentHP);

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(DamageFlash());

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageFlash()
    {
        float timer = 0f;

        while (timer < flashDuration)
        {
            timer += Time.deltaTime;

            float t = timer / flashDuration;

            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = Color.Lerp(flashColor, originalColors[i], t);
            }

            yield return null;
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = originalColors[i];
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " died.");
        Destroy(gameObject);
    }
}
