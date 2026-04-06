using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 1000;
    public float loseDelay = 0.75f;

    public LevelLoader loseLevelLoader;
    public CameraController cameraController;

    private int currentHP;
    private bool isDead = false;
    private PlayerController playerController;

    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;

    private void Awake()
    {
        currentHP = maxHP;
        playerController = GetComponent<PlayerController>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead)
        {
            return;
        }

        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0);

        Debug.Log("Damage taken. Player HP is now: " + currentHP);

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
