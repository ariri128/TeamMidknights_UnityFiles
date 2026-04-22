using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    public GameObject popupPanel;
    public float interactionRange = 2f;

    private bool isOpen = false;

    private void Start()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
    }

    public bool IsPlayerInRange(Transform player)
    {
        if (player == null)
        {
            return false;
        }

        return Vector3.Distance(transform.position, player.position) <= interactionRange;
    }

    public void TogglePopup()
    {
        if (popupPanel == null)
        {
            return;
        }

        isOpen = !isOpen;
        popupPanel.SetActive(isOpen);
    }

    public void ClosePopup()
    {
        if (popupPanel == null)
        {
            return;
        }

        isOpen = false;
        popupPanel.SetActive(false);
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}
