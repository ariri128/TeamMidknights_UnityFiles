using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerObjectInteraction : MonoBehaviour
{
    private InteractiveObject currentInteractable;
    private InteractiveObject openedInteractable;

    private void Update()
    {
        /*
        if (PauseManager.IsPaused)
        {
            return;
        }
        */

        FindClosestInteractable();

        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            HandleInteraction();
        }
    }

    private void FindClosestInteractable()
    {
        InteractiveObject[] interactables = FindObjectsByType<InteractiveObject>(FindObjectsSortMode.None);

        currentInteractable = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < interactables.Length; i++)
        {
            if (interactables[i].IsPlayerInRange(transform))
            {
                float distance = Vector3.Distance(transform.position, interactables[i].transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    currentInteractable = interactables[i];
                }
            }
        }
    }

    private void HandleInteraction()
    {
        if (currentInteractable == null)
        {
            if (openedInteractable != null)
            {
                openedInteractable.ClosePopup();
                openedInteractable = null;
            }

            return;
        }

        if (openedInteractable != null && openedInteractable != currentInteractable)
        {
            openedInteractable.ClosePopup();
        }

        currentInteractable.TogglePopup();

        if (currentInteractable.IsOpen())
        {
            openedInteractable = currentInteractable;
        }
        else
        {
            openedInteractable = null;
        }
    }

    /*
    private InteractiveObject currentInteractable;

    private void Update()
    {
        
        if (PauseManager.IsPaused)
        {
            return;
        }
        

        FindClosestInteractable();

        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            HandleInteraction();
        }
    }

    private void FindClosestInteractable()
    {
        InteractiveObject[] interactables = FindObjectsByType<InteractiveObject>(FindObjectsSortMode.None);

        currentInteractable = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < interactables.Length; i++)
        {
            if (interactables[i].IsPlayerInRange(transform))
            {
                float distance = Vector3.Distance(transform.position, interactables[i].transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    currentInteractable = interactables[i];
                }
            }
        }
    }

    private void HandleInteraction()
    {
        if (currentInteractable == null)
        {
            return;
        }

        currentInteractable.TogglePopup();
    }
    */
}
