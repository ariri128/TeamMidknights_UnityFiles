using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public bool keepCursorVisible = true;

    private void Awake()
    {
        ApplyCursorState();
    }

    private void Start()
    {
        ApplyCursorState();
    }

    private void Update()
    {
        if (keepCursorVisible)
        {
            if (!Cursor.visible || Cursor.lockState != CursorLockMode.None)
            {
                ApplyCursorState();
            }
        }
    }

    private void ApplyCursorState()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
