using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerSpawner : MonoBehaviour
{
    public Transform spawnPoint;

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
        }
    }
}
