using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class TimeRewind : MonoBehaviour
{
    private class PlayerSnapshot
    {
        public Vector3 position;
        public Quaternion rotation;
        public int hp;
        public float timeStamp;

        public PlayerSnapshot(Vector3 position, Quaternion rotation, int hp, float timeStamp)
        {
            this.position = position;
            this.rotation = rotation;
            this.hp = hp;
            this.timeStamp = timeStamp;
        }
    }

    public float rewindSeconds = 5f;
    public float recordInterval = 0.1f;

    public float cooldownDuration = 10f;

    private float cooldownTimer = 0f;

    private List<PlayerSnapshot> history = new List<PlayerSnapshot>();
    private float recordTimer = 0f;

    private CharacterController controller;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        RecordHistory();
        UpdateCooldown();

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryRewind();
        }
    }

    private void UpdateCooldown()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    private void TryRewind()
    {
        if (playerHealth == null || playerHealth.IsDead())
        {
            return;
        }

        if (cooldownTimer > 0f)
        {
            Debug.Log("Rewind on cooldown: " + cooldownTimer.ToString("F1") + "s remaining");
            return;
        }

        bool success = RewindPlayer();

        if (success)
        {
            cooldownTimer = cooldownDuration;
            Debug.Log("Rewind used. Cooldown started (" + cooldownDuration + "s)");
        }
    }

    private void RecordHistory()
    {
        if (playerHealth == null)
        {
            return;
        }

        recordTimer += Time.deltaTime;

        if (recordTimer >= recordInterval)
        {
            recordTimer = 0f;

            PlayerSnapshot snapshot = new PlayerSnapshot(
                transform.position,
                transform.rotation,
                playerHealth.CurrentHP,
                Time.time
            );

            history.Add(snapshot);

            float oldestAllowedTime = Time.time - rewindSeconds - 1f;

            while (history.Count > 0 && history[0].timeStamp < oldestAllowedTime)
            {
                history.RemoveAt(0);
            }
        }
    }

    private bool RewindPlayer()
    {
        float targetTime = Time.time - rewindSeconds;
        PlayerSnapshot rewindSnapshot = null;

        for (int i = history.Count - 1; i >= 0; i--)
        {
            if (history[i].timeStamp <= targetTime)
            {
                rewindSnapshot = history[i];
                break;
            }
        }

        if (rewindSnapshot == null && history.Count > 0)
        {
            rewindSnapshot = history[0];
        }

        if (rewindSnapshot == null)
        {
            Debug.Log("No rewind data available yet.");
            return false;
        }

        if (controller != null)
        {
            controller.enabled = false;
        }

        transform.position = rewindSnapshot.position;
        transform.rotation = rewindSnapshot.rotation;

        if (controller != null)
        {
            controller.enabled = true;
        }

        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.ResetVelocity();
        }

        playerHealth.SetHP(rewindSnapshot.hp);

        Debug.Log("Rewound to position: " + rewindSnapshot.position);
        Debug.Log("Rewound HP to: " + rewindSnapshot.hp);

        return true;
    }

    /*
    private class PlayerSnapshot
    {
        public Vector3 position;
        public Quaternion rotation;
        public int hp;
        public float timeStamp;

        public PlayerSnapshot(Vector3 position, Quaternion rotation, int hp, float timeStamp)
        {
            this.position = position;
            this.rotation = rotation;
            this.hp = hp;
            this.timeStamp = timeStamp;
        }
    }

    public float rewindSeconds = 5f;
    public float recordInterval = 0.1f;

    private List<PlayerSnapshot> history = new List<PlayerSnapshot>();
    private float recordTimer = 0f;

    private CharacterController controller;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        RecordHistory();

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            RewindPlayer();
        }
    }

    private void RecordHistory()
    {
        if (playerHealth == null)
        {
            return;
        }

        recordTimer += Time.deltaTime;

        if (recordTimer >= recordInterval)
        {
            recordTimer = 0f;

            PlayerSnapshot snapshot = new PlayerSnapshot(
                transform.position,
                transform.rotation,
                playerHealth.CurrentHP,
                Time.time
            );

            history.Add(snapshot);

            float oldestAllowedTime = Time.time - rewindSeconds - 1f;

            while (history.Count > 0 && history[0].timeStamp < oldestAllowedTime)
            {
                history.RemoveAt(0);
            }
        }
    }

    private void RewindPlayer()
    {
        if (playerHealth == null || playerHealth.IsDead())
        {
            return;
        }

        float targetTime = Time.time - rewindSeconds;
        PlayerSnapshot rewindSnapshot = null;

        for (int i = history.Count - 1; i >= 0; i--)
        {
            if (history[i].timeStamp <= targetTime)
            {
                rewindSnapshot = history[i];
                break;
            }
        }

        if (rewindSnapshot == null && history.Count > 0)
        {
            rewindSnapshot = history[0];
        }

        if (rewindSnapshot == null)
        {
            Debug.Log("No rewind data available yet.");
            return;
        }

        if (controller != null)
        {
            controller.enabled = false;
        }

        transform.position = rewindSnapshot.position;
        transform.rotation = rewindSnapshot.rotation;

        if (controller != null)
        {
            controller.enabled = true;
        }

        playerHealth.SetHP(rewindSnapshot.hp);

        Debug.Log("Rewound player to position from 5 seconds ago: " + rewindSnapshot.position);
        Debug.Log("Rewound player HP to: " + rewindSnapshot.hp);
    }
    */
}
