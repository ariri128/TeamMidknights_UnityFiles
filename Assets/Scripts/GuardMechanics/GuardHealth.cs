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
    private bool isDead = false;

    private void Awake()
    {
        currentHP = maxHP;

        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].material.color;

        // Register in Awake — only in levels that have a GuardTracker (Level 1)
        if (GuardTracker.Instance != null)
            GuardTracker.Instance.RegisterGuard();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0);

        Debug.Log(gameObject.name + " took damage. Guard HP: " + currentHP);

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(DamageFlash());

        if (currentHP <= 0)
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
        if (isDead) return;
        isDead = true;

        Debug.Log(gameObject.name + " died.");

        // GuardTracker handles dagger spawn, objectives and door check
        GuardTracker.Instance?.ReportGuardDeath(transform.position);

        // Disable AI so guard stops moving
        GuardAI ai = GetComponent<GuardAI>();
        if (ai != null) ai.enabled = false;

        // Play death animation
        GuardAnimationController guardAnim = GetComponent<GuardAnimationController>();
        if (guardAnim != null)
            guardAnim.PlayDeath();
        else
            Destroy(gameObject); // fallback if no animator set up
    }

    public void KillImmediately()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log(gameObject.name + " was defeated by water splash.");
        GuardTracker.Instance?.ReportGuardDeath(transform.position);

        GuardAI ai = GetComponent<GuardAI>();
        if (ai != null) ai.enabled = false;

        GuardAnimationController guardAnim = GetComponent<GuardAnimationController>();
        if (guardAnim != null)
            guardAnim.PlayDeath();
        else
            Destroy(gameObject);
    }
}
