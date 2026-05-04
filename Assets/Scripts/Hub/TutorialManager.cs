using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("Player")]
    public GameObject playerObject;

    [Header("Intro Panel")]
    [Tooltip("Panel shown after the player spawns.")]
    public GameObject introPanel;
    public Button okButton;
    public Button returnToHubFromIntroButton;

    [Tooltip("Seconds after spawn before intro panel appears.")]
    public float spawnDelay = 1.5f;

    [Header("End Panel")]
    [Tooltip("Panel shown after all 16 guards are killed.")]
    public GameObject endPanel;
    public Button restartButton;
    public Button returnToHubFromEndButton;

    [Header("Tutorial UI")]
    [Tooltip("Controls panel shown when player clicks OK.")]
    public GameObject controlsPanel;

    [Tooltip("Any other UI elements (HUD, health bar, etc.) to hide when end panel shows.")]
    public GameObject[] uiToHideOnEnd;

    [Header("Guard Spawning")]
    public GameObject guardPrefab;

    [Tooltip("Spawn points around the arena. Add at least 4-6.")]
    public Transform[] spawnPoints;

    [Tooltip("How many guards spawn per wave.")]
    public int guardsPerWave = 2;

    [Tooltip("Total guards to spawn across all waves.")]
    public int totalGuards = 16;

    [Tooltip("Seconds between waves after previous wave is cleared.")]
    public float waveCooldown = 0.5f;

    [Tooltip("Seconds after OK is clicked before first wave spawns.")]
    public float okDelay = 1.5f;

    [Header("Scene")]
    public string hubSceneName = "Hub";

    // Runtime state
    private int guardsKilled = 0;
    private int guardsAlive = 0;
    private int guardsSpawned = 0;
    private bool tutorialStarted = false;
    private bool tutorialComplete = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (introPanel != null) introPanel.SetActive(false);
        if (endPanel != null) endPanel.SetActive(false);

        if (okButton != null) okButton.onClick.AddListener(OnOkClicked);
        if (returnToHubFromIntroButton != null) returnToHubFromIntroButton.onClick.AddListener(ReturnToHub);
        if (restartButton != null) restartButton.onClick.AddListener(RestartTutorial);
        if (returnToHubFromEndButton != null) returnToHubFromEndButton.onClick.AddListener(ReturnToHub);

        StartCoroutine(ShowIntroPanelAfterDelay());
    }

    // ──────────────────────────────────────────────
    // Intro
    // ──────────────────────────────────────────────

    private IEnumerator ShowIntroPanelAfterDelay()
    {
        yield return new WaitForSeconds(spawnDelay);

        if (introPanel != null)
            introPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SetPlayerMovement(false);
    }

    private void OnOkClicked()
    {
        if (introPanel != null)
            introPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SetPlayerMovement(true);
        SetGodMode(true);

        if (controlsPanel != null)
            controlsPanel.SetActive(true);

        StartCoroutine(StartFirstWaveAfterDelay());
    }

    private IEnumerator StartFirstWaveAfterDelay()
    {
        yield return new WaitForSeconds(okDelay);
        tutorialStarted = true;
        SpawnNextWave();
    }

    // ──────────────────────────────────────────────
    // Guard Spawning
    // ──────────────────────────────────────────────

    private void SpawnNextWave()
    {
        if (guardsSpawned >= totalGuards) return;

        int toSpawn = Mathf.Min(guardsPerWave, totalGuards - guardsSpawned);

        for (int i = 0; i < toSpawn; i++)
        {
            Transform spawnPoint = GetSpawnPoint(i);
            GameObject guard = Instantiate(guardPrefab, spawnPoint.position, spawnPoint.rotation);

            // Assign player reference to GuardAI
            GuardAI ai = guard.GetComponent<GuardAI>();
            if (ai != null)
            {
                ai.player = playerObject.transform;
                // Force immediately into chase state
                ai.ForceChase();
            }

            guardsSpawned++;
            guardsAlive++;
        }

        Debug.Log($"Tutorial: Wave spawned. {guardsSpawned}/{totalGuards} total spawned.");
    }

    private Transform GetSpawnPoint(int index)
    {
        if (spawnPoints.Length == 0) return transform;

        // Spread guards across different spawn points
        int pointIndex = (guardsSpawned + index) % spawnPoints.Length;
        return spawnPoints[pointIndex];
    }

    // ──────────────────────────────────────────────
    // Called by TutorialGuardHealth when a guard dies
    // ──────────────────────────────────────────────

    public void ReportGuardKilled()
    {
        if (!tutorialStarted || tutorialComplete) return;

        guardsAlive--;
        guardsKilled++;

        Debug.Log($"Tutorial: Guard killed. {guardsKilled}/{totalGuards} total killed. {guardsAlive} alive.");

        if (guardsKilled >= totalGuards)
        {
            StartCoroutine(ShowEndPanel());
            return;
        }

        // If current wave is cleared, spawn next wave after cooldown
        if (guardsAlive <= 0)
            StartCoroutine(SpawnNextWaveAfterCooldown());
    }

    private IEnumerator SpawnNextWaveAfterCooldown()
    {
        yield return new WaitForSeconds(waveCooldown);
        SpawnNextWave();
    }

    // ──────────────────────────────────────────────
    // End
    // ──────────────────────────────────────────────

    private IEnumerator ShowEndPanel()
    {
        tutorialComplete = true;

        // Stop player and camera
        SetPlayerMovement(false);
        CameraController cam = FindObjectsByType<CameraController>(FindObjectsSortMode.None)[0];
        if (cam != null) cam.enabled = false;

        yield return new WaitForSeconds(0.5f);

        // Hide controls panel and any other UI
        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        foreach (GameObject ui in uiToHideOnEnd)
            if (ui != null) ui.SetActive(false);

        if (endPanel != null)
            endPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ──────────────────────────────────────────────
    // Buttons
    // ──────────────────────────────────────────────

    private void ReturnToHub()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(hubSceneName, LoadSceneMode.Single);
    }

    private void RestartTutorial()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    private void SetPlayerMovement(bool enabled)
    {
        if (playerObject == null) return;
        var controller = playerObject.GetComponent<PlayerController>();
        if (controller != null) controller.enabled = enabled;
    }

    private void SetGodMode(bool on)
    {
        if (playerObject == null) return;

        // Disable damage by disabling PlayerHealth's TakeDamage calls
        PlayerHealth health = playerObject.GetComponent<PlayerHealth>();
        if (health != null) health.godMode = on;

        // Disable mana consumption
        PlayerMana mana = playerObject.GetComponent<PlayerMana>();
        if (mana != null) mana.infiniteMana = on;
    }

    /*
    public static TutorialManager Instance { get; private set; }

    [Header("Player")]
    public GameObject playerObject;

    [Header("Intro Panel")]
    [Tooltip("Panel shown after the player spawns.")]
    public GameObject introPanel;
    public Button okButton;
    public Button returnToHubFromIntroButton;

    [Tooltip("Seconds after spawn before intro panel appears.")]
    public float spawnDelay = 1.5f;

    [Header("End Panel")]
    [Tooltip("Panel shown after all 16 guards are killed.")]
    public GameObject endPanel;
    public Button restartButton;
    public Button returnToHubFromEndButton;

    [Header("Guard Spawning")]
    public GameObject guardPrefab;

    [Tooltip("Spawn points around the arena. Add at least 4-6.")]
    public Transform[] spawnPoints;

    [Tooltip("How many guards spawn per wave.")]
    public int guardsPerWave = 2;

    [Tooltip("Total guards to spawn across all waves.")]
    public int totalGuards = 16;

    [Tooltip("Seconds between waves after previous wave is cleared.")]
    public float waveCooldown = 0.5f;

    [Tooltip("Seconds after OK is clicked before first wave spawns.")]
    public float okDelay = 1.5f;

    [Header("Scene")]
    public string hubSceneName = "Hub";

    // Runtime state
    private int guardsKilled = 0;
    private int guardsAlive = 0;
    private int guardsSpawned = 0;
    private bool tutorialStarted = false;
    private bool tutorialComplete = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (introPanel != null) introPanel.SetActive(false);
        if (endPanel != null) endPanel.SetActive(false);

        if (okButton != null) okButton.onClick.AddListener(OnOkClicked);
        if (returnToHubFromIntroButton != null) returnToHubFromIntroButton.onClick.AddListener(ReturnToHub);
        if (restartButton != null) restartButton.onClick.AddListener(RestartTutorial);
        if (returnToHubFromEndButton != null) returnToHubFromEndButton.onClick.AddListener(ReturnToHub);

        StartCoroutine(ShowIntroPanelAfterDelay());
    }

    // ──────────────────────────────────────────────
    // Intro
    // ──────────────────────────────────────────────

    private IEnumerator ShowIntroPanelAfterDelay()
    {
        yield return new WaitForSeconds(spawnDelay);

        if (introPanel != null)
            introPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SetPlayerMovement(false);
    }

    private void OnOkClicked()
    {
        if (introPanel != null)
            introPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SetPlayerMovement(true);
        SetGodMode(true);

        StartCoroutine(StartFirstWaveAfterDelay());
    }

    private IEnumerator StartFirstWaveAfterDelay()
    {
        yield return new WaitForSeconds(okDelay);
        tutorialStarted = true;
        SpawnNextWave();
    }

    // ──────────────────────────────────────────────
    // Guard Spawning
    // ──────────────────────────────────────────────

    private void SpawnNextWave()
    {
        if (guardsSpawned >= totalGuards) return;

        int toSpawn = Mathf.Min(guardsPerWave, totalGuards - guardsSpawned);

        for (int i = 0; i < toSpawn; i++)
        {
            Transform spawnPoint = GetSpawnPoint(i);
            GameObject guard = Instantiate(guardPrefab, spawnPoint.position, spawnPoint.rotation);

            // Assign player reference to GuardAI
            GuardAI ai = guard.GetComponent<GuardAI>();
            if (ai != null)
            {
                ai.player = playerObject.transform;
                // Force immediately into chase state
                ai.ForceChase();
            }

            guardsSpawned++;
            guardsAlive++;
        }

        Debug.Log($"Tutorial: Wave spawned. {guardsSpawned}/{totalGuards} total spawned.");
    }

    private Transform GetSpawnPoint(int index)
    {
        if (spawnPoints.Length == 0) return transform;

        // Spread guards across different spawn points
        int pointIndex = (guardsSpawned + index) % spawnPoints.Length;
        return spawnPoints[pointIndex];
    }

    // ──────────────────────────────────────────────
    // Called by TutorialGuardHealth when a guard dies
    // ──────────────────────────────────────────────

    public void ReportGuardKilled()
    {
        if (!tutorialStarted || tutorialComplete) return;

        guardsAlive--;
        guardsKilled++;

        Debug.Log($"Tutorial: Guard killed. {guardsKilled}/{totalGuards} total killed. {guardsAlive} alive.");

        if (guardsKilled >= totalGuards)
        {
            StartCoroutine(ShowEndPanel());
            return;
        }

        // If current wave is cleared, spawn next wave after cooldown
        if (guardsAlive <= 0)
            StartCoroutine(SpawnNextWaveAfterCooldown());
    }

    private IEnumerator SpawnNextWaveAfterCooldown()
    {
        yield return new WaitForSeconds(waveCooldown);
        SpawnNextWave();
    }

    // ──────────────────────────────────────────────
    // End
    // ──────────────────────────────────────────────

    private IEnumerator ShowEndPanel()
    {
        tutorialComplete = true;

        // Stop player and camera
        SetPlayerMovement(false);
        CameraController cam = FindObjectsByType<CameraController>(FindObjectsSortMode.None)[0];
        if (cam != null) cam.enabled = false;

        yield return new WaitForSeconds(0.5f);

        if (endPanel != null)
            endPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ──────────────────────────────────────────────
    // Buttons
    // ──────────────────────────────────────────────

    private void ReturnToHub()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(hubSceneName, LoadSceneMode.Single);
    }

    private void RestartTutorial()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    private void SetPlayerMovement(bool enabled)
    {
        if (playerObject == null) return;
        var controller = playerObject.GetComponent<PlayerController>();
        if (controller != null) controller.enabled = enabled;
    }

    private void SetGodMode(bool on)
    {
        if (playerObject == null) return;

        // Disable damage by disabling PlayerHealth's TakeDamage calls
        PlayerHealth health = playerObject.GetComponent<PlayerHealth>();
        if (health != null) health.godMode = on;

        // Disable mana consumption
        PlayerMana mana = playerObject.GetComponent<PlayerMana>();
        if (mana != null) mana.infiniteMana = on;
    }
    */

    /*
    public static TutorialManager Instance { get; private set; }

    [Header("Player")]
    public GameObject playerObject;

    [Header("Intro Panel")]
    [Tooltip("Panel shown after the player spawns.")]
    public GameObject introPanel;
    public Button okButton;
    public Button returnToHubFromIntroButton;

    [Tooltip("Seconds after spawn before intro panel appears.")]
    public float spawnDelay = 1.5f;

    [Header("End Panel")]
    [Tooltip("Panel shown after all 16 guards are killed.")]
    public GameObject endPanel;
    public Button restartButton;
    public Button returnToHubFromEndButton;

    [Header("Guard Spawning")]
    public GameObject guardPrefab;

    [Tooltip("Spawn points around the arena. Add at least 4-6.")]
    public Transform[] spawnPoints;

    [Tooltip("How many guards spawn per wave.")]
    public int guardsPerWave = 2;

    [Tooltip("Total guards to spawn across all waves.")]
    public int totalGuards = 16;

    [Tooltip("Seconds between waves after previous wave is cleared.")]
    public float waveCooldown = 0.5f;

    [Tooltip("Seconds after OK is clicked before first wave spawns.")]
    public float okDelay = 1.5f;

    [Header("Scene")]
    public string hubSceneName = "Hub";

    // Runtime state
    private int guardsKilled = 0;
    private int guardsAlive = 0;
    private int guardsSpawned = 0;
    private bool tutorialStarted = false;
    private bool tutorialComplete = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (introPanel != null) introPanel.SetActive(false);
        if (endPanel != null) endPanel.SetActive(false);

        if (okButton != null) okButton.onClick.AddListener(OnOkClicked);
        if (returnToHubFromIntroButton != null) returnToHubFromIntroButton.onClick.AddListener(ReturnToHub);
        if (restartButton != null) restartButton.onClick.AddListener(RestartTutorial);
        if (returnToHubFromEndButton != null) returnToHubFromEndButton.onClick.AddListener(ReturnToHub);

        StartCoroutine(ShowIntroPanelAfterDelay());
    }

    // ──────────────────────────────────────────────
    // Intro
    // ──────────────────────────────────────────────

    private IEnumerator ShowIntroPanelAfterDelay()
    {
        yield return new WaitForSeconds(spawnDelay);

        if (introPanel != null)
            introPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SetPlayerMovement(false);
    }

    private void OnOkClicked()
    {
        if (introPanel != null)
            introPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SetPlayerMovement(true);

        StartCoroutine(StartFirstWaveAfterDelay());
    }

    private IEnumerator StartFirstWaveAfterDelay()
    {
        yield return new WaitForSeconds(okDelay);
        tutorialStarted = true;
        SpawnNextWave();
    }

    // ──────────────────────────────────────────────
    // Guard Spawning
    // ──────────────────────────────────────────────

    private void SpawnNextWave()
    {
        if (guardsSpawned >= totalGuards) return;

        int toSpawn = Mathf.Min(guardsPerWave, totalGuards - guardsSpawned);

        for (int i = 0; i < toSpawn; i++)
        {
            Transform spawnPoint = GetSpawnPoint(i);
            GameObject guard = Instantiate(guardPrefab, spawnPoint.position, spawnPoint.rotation);

            // Assign player reference to GuardAI
            GuardAI ai = guard.GetComponent<GuardAI>();
            if (ai != null)
            {
                ai.player = playerObject.transform;
                // Force immediately into chase state
                ai.ForceChase();
            }

            guardsSpawned++;
            guardsAlive++;
        }

        Debug.Log($"Tutorial: Wave spawned. {guardsSpawned}/{totalGuards} total spawned.");
    }

    private Transform GetSpawnPoint(int index)
    {
        if (spawnPoints.Length == 0) return transform;

        // Spread guards across different spawn points
        int pointIndex = (guardsSpawned + index) % spawnPoints.Length;
        return spawnPoints[pointIndex];
    }

    // ──────────────────────────────────────────────
    // Called by TutorialGuardHealth when a guard dies
    // ──────────────────────────────────────────────

    public void ReportGuardKilled()
    {
        if (!tutorialStarted || tutorialComplete) return;

        guardsAlive--;
        guardsKilled++;

        Debug.Log($"Tutorial: Guard killed. {guardsKilled}/{totalGuards} total killed. {guardsAlive} alive.");

        if (guardsKilled >= totalGuards)
        {
            StartCoroutine(ShowEndPanel());
            return;
        }

        // If current wave is cleared, spawn next wave after cooldown
        if (guardsAlive <= 0)
            StartCoroutine(SpawnNextWaveAfterCooldown());
    }

    private IEnumerator SpawnNextWaveAfterCooldown()
    {
        yield return new WaitForSeconds(waveCooldown);
        SpawnNextWave();
    }

    // ──────────────────────────────────────────────
    // End
    // ──────────────────────────────────────────────

    private IEnumerator ShowEndPanel()
    {
        tutorialComplete = true;

        // Stop player and camera
        SetPlayerMovement(false);
        CameraController cam = FindObjectsByType<CameraController>(FindObjectsSortMode.None)[0];
        if (cam != null) cam.enabled = false;

        yield return new WaitForSeconds(0.5f);

        if (endPanel != null)
            endPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ──────────────────────────────────────────────
    // Buttons
    // ──────────────────────────────────────────────

    private void ReturnToHub()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(hubSceneName, LoadSceneMode.Single);
    }

    private void RestartTutorial()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    private void SetPlayerMovement(bool enabled)
    {
        if (playerObject == null) return;
        var controller = playerObject.GetComponent<PlayerController>();
        if (controller != null) controller.enabled = enabled;
    }
    */
}
