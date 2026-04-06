using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class TimeSlow : MonoBehaviour
{
    public float slowDuration = 3f;
    public float cooldownDuration = 10f;
    public float guardSpeedMultiplier = 0.4f;

    private bool isSlowActive = false;
    private float cooldownTimer = 0f;

    private void Update()
    {
        UpdateCooldown();

        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            TryActivateTimeSlow();
        }
    }

    private void UpdateCooldown()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    private void TryActivateTimeSlow()
    {
        if (isSlowActive)
        {
            Debug.Log("Time slow is already active.");
            return;
        }

        if (cooldownTimer > 0f)
        {
            Debug.Log("Time slow on cooldown: " + cooldownTimer.ToString("F1") + "s remaining");
            return;
        }

        StartCoroutine(ActivateTimeSlow());
    }

    private IEnumerator ActivateTimeSlow()
    {
        isSlowActive = true;
        cooldownTimer = cooldownDuration;

        GuardAI[] guards = FindObjectsByType<GuardAI>(FindObjectsSortMode.None);

        for (int i = 0; i < guards.Length; i++)
        {
            guards[i].ApplySlow(guardSpeedMultiplier);
        }

        Debug.Log("Time slow activated.");

        yield return new WaitForSeconds(slowDuration);

        for (int i = 0; i < guards.Length; i++)
        {
            if (guards[i] != null)
            {
                guards[i].RestoreNormalSpeed();
            }
        }

        isSlowActive = false;
        Debug.Log("Time slow ended.");
    }
}
