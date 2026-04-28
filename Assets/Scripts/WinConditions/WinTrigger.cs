using UnityEngine;
using System.Collections;

public class WinTrigger : MonoBehaviour
{
    public LevelLoader levelLoader;
    public float loadDelay = 0.75f;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
        {
            return;
        }

        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            hasTriggered = true;
            player.DisableMovement();
            StartCoroutine(LoadWinSceneAfterDelay());
        }
    }

    private IEnumerator LoadWinSceneAfterDelay()
    {
        yield return new WaitForSeconds(loadDelay);

        if (levelLoader != null)
        {
            levelLoader.LoadNextLevel();
        }
        else
        {
            Debug.LogError("WinTrigger is missing a LevelLoader reference.");
        }
    }
}
