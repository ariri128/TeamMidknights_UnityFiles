using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerSpawner : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject spawnSplashPrefab;
    public float splashGroundRayDistance = 5f;

    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (spawnPoint != null)
        {
            if (controller != null)
            {
                controller.enabled = false;
            }

            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;

            if (controller != null)
            {
                controller.enabled = true;
            }

            SpawnSplash(transform.position);
        }
    }

    private void SpawnSplash(Vector3 position)
    {
        if (spawnSplashPrefab == null)
        {
            return;
        }

        Vector3 splashPosition = position;

        if (Physics.Raycast(position + Vector3.up * 1f, Vector3.down, out RaycastHit hit, splashGroundRayDistance))
        {
            splashPosition = hit.point;
        }

        Instantiate(spawnSplashPrefab, splashPosition, Quaternion.identity);
    }
}
