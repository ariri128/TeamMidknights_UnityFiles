using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 1000;

    [Tooltip("If true, player cannot take damage (used in tutorial).")]
    public bool godMode = false;
    public float loseDelay = 2.3f;

    public LevelLoader loseLevelLoader;
    public CameraController cameraController;

    private int currentHP;
    private bool isDead = false;
    private PlayerController playerController;

    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;

    public int regenAmount = 5;
    public float regenInterval = 1f;
    public float regenDelayAfterDamage = 3f;

    private float regenTimer;
    private float lastDamageTime;

    private void Awake()
    {
        currentHP = maxHP;
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        RegenerateHealth();
    }

    private void RegenerateHealth()
    {
        if (isDead)
        {
            return;
        }

        if (currentHP >= maxHP)
        {
            return;
        }

        if (Time.time < lastDamageTime + regenDelayAfterDamage)
        {
            return;
        }

        regenTimer += Time.deltaTime;

        if (regenTimer >= regenInterval)
        {
            regenTimer = 0f;
            currentHP += regenAmount;
            currentHP = Mathf.Min(currentHP, maxHP);

            Debug.Log("HP regenerated. HP is now: " + currentHP);
        }
    }

    public void TakeDamage(int amount)
    {
        if (godMode) return;
        if (isDead)
        {
            return;
        }

        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0);

        lastDamageTime = Time.time;

        Debug.Log("Damage taken. Player HP is now: " + currentHP);

        if (ObjectiveUpdateUI.Instance != null)
        {
            ObjectiveUpdateUI.Instance.ShowMessage("Damage Taken. HP is now " + currentHP);
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        // Disable all player mechanics
        if (playerController != null)
            playerController.DisableMovement();

        // Disable attack and time mechanics so player can't do anything
        PlayerAttack attack = GetComponent<PlayerAttack>();
        if (attack != null) attack.enabled = false;

        TimeSlow timeSlow = GetComponent<TimeSlow>();
        if (timeSlow != null) timeSlow.enabled = false;

        TimeRewind timeRewind = GetComponent<TimeRewind>();
        if (timeRewind != null) timeRewind.enabled = false;

        // Push player back away from guards so dying animation has space
        PushPlayerBackFromGuards();

        // Stop all guard movement and reset to idle animation
        GuardAI[] guards = FindObjectsByType<GuardAI>(FindObjectsSortMode.None);
        foreach (GuardAI guard in guards)
        {
            guard.enabled = false;
            GuardAnimationController guardAnim = guard.GetComponent<GuardAnimationController>();
            if (guardAnim != null)
                guardAnim.SetIdle();
        }

        PlayerAnimationController playerAnimation = GetComponent<PlayerAnimationController>();
        if (playerAnimation != null)
            playerAnimation.PlayDying();

        if (cameraController != null)
            cameraController.enabled = false;

        StartCoroutine(LoadLoseSceneAfterDelay());
    }

    private void PushPlayerBackFromGuards()
    {
        GuardAI[] guards = FindObjectsByType<GuardAI>(FindObjectsSortMode.None);
        if (guards.Length == 0) return;

        Vector3 pushDirection = Vector3.zero;

        foreach (GuardAI guard in guards)
        {
            Vector3 awayFromGuard = transform.position - guard.transform.position;
            awayFromGuard.y = 0f;
            if (awayFromGuard.magnitude > 0.01f)
                pushDirection += awayFromGuard.normalized;
        }

        if (pushDirection.magnitude < 0.01f) return;

        pushDirection.Normalize();

        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
            cc.Move(pushDirection * 1.5f);
        else
            transform.position += pushDirection * 1.5f;
    }

    public void SetHP(int value)
    {
        if (isDead)
        {
            return;
        }

        currentHP = Mathf.Clamp(value, 0, maxHP);
        Debug.Log("HP restored to: " + currentHP);

        if (ObjectiveUpdateUI.Instance != null)
        {
            ObjectiveUpdateUI.Instance.ShowMessage("HP restored to " + currentHP);
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    private IEnumerator LoadLoseSceneAfterDelay()
    {
        // Try to get the actual Dying animation length from the animator
        float delay = loseDelay;

        PlayerAnimationController animController = GetComponent<PlayerAnimationController>();
        if (animController != null && animController.animator != null)
        {
            // Wait until the Dying state is entered, then wait for it to finish
            float timeout = loseDelay + 2f;
            float elapsed = 0f;
            bool enteredDying = false;

            while (elapsed < timeout)
            {
                AnimatorStateInfo info = animController.animator.GetCurrentAnimatorStateInfo(0);

                if (info.IsName("Dying"))
                    enteredDying = true;

                if (enteredDying && info.IsName("Dying") && info.normalizedTime >= 0.98f)
                    break;

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Small buffer after animation ends before loading scene
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            yield return new WaitForSeconds(loseDelay);
        }

        if (loseLevelLoader != null)
        {
            loseLevelLoader.LoadNextLevel();
        }
        else
        {
            Debug.LogError("PlayerHealth is missing a LevelLoader reference.");
        }
    }
}
