using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class TimeSlow : MonoBehaviour
{
    public float slowDuration = 3f;
    public float cooldownDuration = 10f;
    public float guardSpeedMultiplier = 0.4f;
    public int manaCost = 10;

    private bool isSlowActive = false;
    private float cooldownTimer = 0f;

    public bool IsSlowActive => isSlowActive;

    public bool IsEntryAnimationPlaying
    {
        get
        {
            PlayerAnimationController anim = GetComponent<PlayerAnimationController>();
            return anim != null && anim.IsPauseTimePlaying();
        }
    }

    private PlayerMana playerMana;

    private void Awake()
    {
        playerMana = GetComponent<PlayerMana>();
    }

    private void Update()
    {
        UpdateCooldown();

        if (Keyboard.current != null && (Keyboard.current.leftShiftKey.wasPressedThisFrame || Keyboard.current.rightShiftKey.wasPressedThisFrame))
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

        if (playerMana == null || !playerMana.TrySpendMana(manaCost))
        {
            return;
        }

        StartCoroutine(ActivateTimeSlow());
    }

    private IEnumerator ActivateTimeSlow()
    {
        isSlowActive = true;

        PlayerAnimationController playerAnimation = GetComponent<PlayerAnimationController>();
        if (playerAnimation != null)
        {
            playerAnimation.PlayPauseTime();

             //Added in sound here!
            Debug.Log("Played Sound");
            AudioManager.Instance.Play(AudioManager.SoundType.SlowDownTime);
        }

        GuardAI[] guards = FindObjectsByType<GuardAI>(FindObjectsSortMode.None);

        for (int i = 0; i < guards.Length; i++)
        {
            guards[i].ApplySlow(guardSpeedMultiplier);
        }

        Debug.Log("Time slow activated.");

        if (ObjectiveUpdateUI.Instance != null)
        {
            ObjectiveUpdateUI.Instance.ShowMessage("Time Slowed Down");
        }

        yield return new WaitForSeconds(slowDuration);

        for (int i = 0; i < guards.Length; i++)
        {
            if (guards[i] != null)
            {
                guards[i].RestoreNormalSpeed();
            }
        }

        isSlowActive = false;
        cooldownTimer = cooldownDuration;

        Debug.Log("Time slow ended.");
    }
}
