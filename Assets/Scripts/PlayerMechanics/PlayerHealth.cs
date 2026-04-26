using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 1000;
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

        if (playerController != null)
        {
            playerController.DisableMovement();
        }

        PlayerAnimationController playerAnimation = GetComponent<PlayerAnimationController>();
        if (playerAnimation != null)
        {
            playerAnimation.PlayDying();
        }

        if (cameraController != null)
        {
            cameraController.enabled = false;
        }

        StartCoroutine(LoadLoseSceneAfterDelay());
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
        yield return new WaitForSeconds(loseDelay);

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
