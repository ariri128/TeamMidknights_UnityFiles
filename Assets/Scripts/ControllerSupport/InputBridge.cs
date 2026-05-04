using UnityEngine;

public class InputBridge : MonoBehaviour
{
    public static event System.Action OnInteractPressed;

    public static void FireInteract()
    {
        OnInteractPressed?.Invoke();
    }
}
